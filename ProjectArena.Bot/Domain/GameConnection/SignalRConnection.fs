module ProjectArena.Bot.Domain.GameConnection.SignalRConnection
open Microsoft.AspNetCore.SignalR.Client
open System.Threading.Tasks
open ProjectArena.Bot.Models.States
open ProjectArena.Bot.Models.Dtos
open System.Threading
open System
open Microsoft.AspNetCore.Http.Connections.Client
open System.Net
open Microsoft.Extensions.Logging
open ProjectArena.Bot.Helpers.MappingHelper

let private fillHubOptions (auth: string) (url: string) (options: HttpConnectionOptions) =
    options.Cookies <- CookieContainer()
    options.Cookies.Add(Uri(url), Cookie("Authorization", auth))
    ()

let createConnection (logger: ILogger<unit>) (tokenSource: CancellationTokenSource) (auth: string) (url: string) =
    let connection =
        HubConnectionBuilder()
            .WithUrl(url, fillHubOptions auth url)
            .WithAutomaticReconnect()
            .ConfigureLogging(fun logging -> 
                logging.AddConsole().SetMinimumLevel(LogLevel.Error) |> ignore)
            .Build()
    connection.add_Closed(fun error ->
        logger.LogError (sprintf "Hub connection is lost")
        Task.CompletedTask)
    connection.On("BattleSynchronizationError", fun () ->
        logger.LogError (sprintf "Synchronization error")) |> ignore
    connection

let startConnection (connection: HubConnection) =
    connection.StartAsync() |> Async.AwaitTask |> Async.RunSynchronously
    connection

let subscribeOnScene (connection: HubConnection) (func: (SynchronizationAction * SynchronizerDto) -> unit) =
    connection.On("BattleStartGame", fun (synchronizer: ProjectArena.Infrastructure.Models.Battle.Synchronization.SynchronizerDto) -> func(StartGame, mapSynchronizer synchronizer)) |> ignore
    connection.On("BattleMove", fun (synchronizer: ProjectArena.Infrastructure.Models.Battle.Synchronization.SynchronizerDto) -> func(Move, mapSynchronizer synchronizer)) |> ignore
    connection.On("BattleAttack", fun (synchronizer: ProjectArena.Infrastructure.Models.Battle.Synchronization.SynchronizerDto) -> func(Attack, mapSynchronizer synchronizer)) |> ignore
    connection.On("BattleCast", fun (synchronizer: ProjectArena.Infrastructure.Models.Battle.Synchronization.SynchronizerDto) -> func(Cast, mapSynchronizer synchronizer)) |> ignore
    connection.On("BattleDecoration", fun (synchronizer: ProjectArena.Infrastructure.Models.Battle.Synchronization.SynchronizerDto) -> func(Decoration, mapSynchronizer synchronizer)) |> ignore
    connection.On("BattleEndTurn", fun (synchronizer: ProjectArena.Infrastructure.Models.Battle.Synchronization.SynchronizerDto) -> func(EndTurn, mapSynchronizer synchronizer)) |> ignore
    connection.On("BattleEndGame", fun (synchronizer: ProjectArena.Infrastructure.Models.Battle.Synchronization.SynchronizerDto) -> func(EndGame, mapSynchronizer synchronizer)) |> ignore
    connection.On("BattleLeave", fun (synchronizer: ProjectArena.Infrastructure.Models.Battle.Synchronization.SynchronizerDto) -> func(Leave, mapSynchronizer synchronizer)) |> ignore
    connection.On("BattleNoActorsDraw", fun (synchronizer: ProjectArena.Infrastructure.Models.Battle.Synchronization.SynchronizerDto) -> func(NoActorsDraw, mapSynchronizer synchronizer)) |> ignore

let orderAttack (connection: HubConnection) (sceneId: Guid, actorId: int, targetX: int, targetY: int) =
    connection.SendAsync("OrderAttackAsync", sceneId, actorId, targetX, targetY) |> Async.AwaitTask |> Async.Start
    
let orderMove (connection: HubConnection) (sceneId: Guid, actorId: int, targetX: int, targetY: int) =
    connection.SendAsync("OrderMoveAsync", sceneId, actorId, targetX, targetY) |> Async.AwaitTask |> Async.Start

let orderCast (connection: HubConnection) (sceneId: Guid, actorId: int, skillId: int, targetX: int, targetY: int) =
    connection.SendAsync("OrderCastAsync", sceneId, actorId, skillId, targetX, targetY) |> Async.AwaitTask |> Async.Start