module ProjectArena.Bot.Domain.GameConnection.SignalRConnection
open Microsoft.AspNetCore.SignalR.Client
open System.Threading.Tasks
open ProjectArena.Bot.Models.States
open ProjectArena.Bot.Models.Dtos
open System.Threading
open System


let openConnection (tokenSource: CancellationTokenSource) (url: string) =
    let connection =
        HubConnectionBuilder()
            .WithUrl(url)
            .Build()
    connection.add_Closed(fun error ->
        printfn "Hub connection is lost"
        tokenSource.Cancel()
        Task.CompletedTask)
    connection.On("BattleSynchronizationError", fun () ->
        printfn "Synchronization error") |> ignore
    connection

let subscribeOnScene (connection: HubConnection) (func: (SynchronizationAction * SynchronizerDto) -> unit) =
    connection.On("BattleStartGame", fun (synchronizer: SynchronizerDto) -> func(StartGame, synchronizer)) |> ignore
    connection.On("BattleMove", fun (synchronizer: SynchronizerDto) -> func(Move, synchronizer)) |> ignore
    connection.On("BattleAttack", fun (synchronizer: SynchronizerDto) -> func(Attack, synchronizer)) |> ignore
    connection.On("BattleCast", fun (synchronizer: SynchronizerDto) -> func(Cast, synchronizer)) |> ignore
    connection.On("BattleWait", fun (synchronizer: SynchronizerDto) -> func(Wait, synchronizer)) |> ignore
    connection.On("BattleDecoration", fun (synchronizer: SynchronizerDto) -> func(Decoration, synchronizer)) |> ignore
    connection.On("BattleEndTurn", fun (synchronizer: SynchronizerDto) -> func(EndTurn, synchronizer)) |> ignore
    connection.On("BattleEndGame", fun (synchronizer: SynchronizerDto) -> func(EndGame, synchronizer)) |> ignore
    connection.On("BattleSkipTurn", fun (synchronizer: SynchronizerDto) -> func(SkipTurn, synchronizer)) |> ignore
    connection.On("BattleLeave", fun (synchronizer: SynchronizerDto) -> func(Leave, synchronizer)) |> ignore
    connection.On("BattleNoActorsDraw", fun (synchronizer: SynchronizerDto) -> func(NoActorsDraw, synchronizer)) |> ignore

let orderAttack (connection: HubConnection) (sceneId: Guid, actorId: int, targetX: int, targetY: int) =
    connection.SendAsync("OrderAttackAsync", sceneId, actorId, targetX, targetY) |> Async.AwaitTask |> Async.Start
    
let orderMove (connection: HubConnection) (sceneId: Guid, actorId: int, targetX: int, targetY: int) =
    connection.SendAsync("OrderMoveAsync", sceneId, actorId, targetX, targetY) |> Async.AwaitTask |> Async.Start

let orderCast (connection: HubConnection) (sceneId: Guid, actorId: int, skillId: int, targetX: int, targetY: int) =
    connection.SendAsync("OrderCastAsync", sceneId, actorId, skillId, targetX, targetY) |> Async.AwaitTask |> Async.Start
    
let orderWait (connection: HubConnection) (sceneId: Guid, actorId: int) =
    connection.SendAsync("OrderWaitAsync", sceneId, actorId) |> Async.AwaitTask |> Async.Start