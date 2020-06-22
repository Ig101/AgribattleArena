module ProjectArena.Bot.Processors.GameConnectionProcessor
open System.Threading
open ProjectArena.Bot.Models.Configuration
open ProjectArena.Bot.Domain.GameConnection.SignalRConnection
open ProjectArena.Bot.Domain.SceneStateWorker
open ProjectArena.Bot.Models.States
open Microsoft.AspNetCore.SignalR.Client
open Microsoft.Extensions.Logging

let private subscribe (worker: SceneStateWorker) (hubConnection: HubConnection) =
    subscribeOnScene hubConnection (fun (action, synchronizer) ->
        worker.SendNewMessage {
            Action = action
            Synchronizer = synchronizer
        }
    )
    hubConnection

let private initializeWorker (configuration: RawConfigurationWithStorageAndUser) () =
    configuration.Logger.LogInformation "Loading Worker..."
    let worker = SceneStateWorker.Create configuration.Logger
    let tokenSource = new CancellationTokenSource()
    (worker, tokenSource)

let private initializeHubConnection (configuration: RawConfigurationWithStorageAndUser) (worker: SceneStateWorker, tokenSource: CancellationTokenSource) =
    configuration.Logger.LogInformation "Loading Connection..."
    let connection =
        createConnection configuration.Logger tokenSource configuration.User.AuthCookie (sprintf "%s/%s" configuration.ApiHost configuration.HubPath)
        |> subscribe worker 
        |> startConnection
    (worker, tokenSource, connection)

let setupGameConnection (configuration: RawConfigurationWithStorageAndUser) =
    let worker, tokenSource, hub =
        ()
        |> initializeWorker configuration
        |> initializeHubConnection configuration
    configuration.Logger.LogInformation "Loading finished."
    {
        Logger = configuration.Logger
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
