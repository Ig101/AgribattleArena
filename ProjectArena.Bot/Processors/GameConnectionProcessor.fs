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

let private authorize (configuration: ApiConfiguration) =
    printfn "Authorizing..."
    configuration.Host
    |> authorize configuration.Login configuration.Password
    |> Async.RunSynchronously
    configuration
  
let private getUserId (configuration: ApiConfiguration) =
    printfn "Loading User info..."
    let userId =
        configuration.Host
        |> getUserInfo
        |> Async.RunSynchronously
    (configuration, userId.Id)

let private subscribe (worker: SceneStateWorker) (hubConnection: HubConnection) =
    subscribeOnScene hubConnection (fun (action, synchronizer) ->
        worker.SendNewMessage {
            Action = action
            Synchronizer = synchronizer
        }
    )
    hubConnection

let private initializeWorker (configuration: ApiConfiguration, userId: string) =
    printfn "Loading Worker..."
    let worker = SceneStateWorker.Unit()
    let tokenSource = new CancellationTokenSource()
    (worker, tokenSource, configuration, userId)

let private initializeHubConnection (worker: SceneStateWorker, tokenSource: CancellationTokenSource, configuration: ApiConfiguration, userId: string) =
    printfn "Loading Connection..."
    let connection =
        openConnection tokenSource (sprintf "%s/%s" configuration.Host configuration.HubPath)
        |> subscribe worker 
    (worker, tokenSource, connection, userId)

let setupGameConnection (configuration: RawConfigurationWithStorageConnection) =
    let worker, tokenSource, hub, userId =
        configuration.Api
        |> authorize
        |> getUserId
        |> initializeWorker
        |> initializeHubConnection
    printfn "Loading finished."
    {
        Learning = configuration.Learning
        ApiHost = configuration.Api.Host
        UserId = userId
        Hub = hub
        Storage = configuration.Storage
        Worker = worker
        WorkerCancellationToken = tokenSource.Token
    }

let dispose (configuration: Configuration) =
    configuration.Hub.DisposeAsync().Wait()
    ()
