module ProjectArena.Bot.Processors.GameConnectionProcessor
open System.Threading
open ProjectArena.Bot.Models.Configuration
open ProjectArena.Bot.Domain.GameConnection.SignalRConnection
open ProjectArena.Bot.Domain.GameConnection.GameApi
open ProjectArena.Bot.Domain.SceneStateWorker
open ProjectArena.Bot.Models.States
open Microsoft.AspNetCore.SignalR.Client
open FSharp.Control
open ProjectArena.Infrastructure.Mongo

let private subscribe (worker: SceneStateWorker) (hubConnection: HubConnection) =
    subscribeOnScene hubConnection (fun (action, synchronizer) ->
        worker.SendNewMessage {
            Action = action
            Synchronizer = synchronizer
        }
    )
    hubConnection

let private initializeWorker () =
    printfn "Loading Worker..."
    let worker = SceneStateWorker.Unit()
    let tokenSource = new CancellationTokenSource()
    (worker, tokenSource)

let private initializeHubConnection (host: string, hubPath: string) (auth: string) (worker: SceneStateWorker, tokenSource: CancellationTokenSource) =
    printfn "Loading Connection..."
    let connection =
        createConnection tokenSource auth (sprintf "%s/%s" host hubPath)
        |> subscribe worker 
        |> startConnection
    (worker, tokenSource, connection)

let setupGameConnection (configuration: RawConfigurationWithStorageAndUser) =
    let worker, tokenSource, hub =
        ()
        |> initializeWorker
        |> initializeHubConnection (configuration.ApiHost, configuration.HubPath) configuration.User.AuthCookie
    printfn "Loading finished."
    {
        Learning = configuration.Learning
        ApiHost = configuration.ApiHost
        User = configuration.User
        Hub = hub
        Storage = configuration.Storage
        Worker = worker
        WorkerCancellationToken = tokenSource.Token
    }

let dispose (configuration: Configuration) =
    configuration.Hub.DisposeAsync().Wait()
    ()
