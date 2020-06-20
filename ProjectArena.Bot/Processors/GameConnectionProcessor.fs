module ProjectArena.Bot.Processors.GameConnectionProcessor
open System.Threading
open ProjectArena.Bot.Models.Configuration
open ProjectArena.Bot.Domain.GameConnection.SignalRConnection
open ProjectArena.Bot.Domain.GameConnection.GameApi
open ProjectArena.Bot.Domain.SceneStateWorker
open ProjectArena.Bot.Models.States
open Microsoft.AspNetCore.SignalR.Client
open FSharp.Control

let private authorize (configuration: ApiConfiguration) =
    authorize configuration |> Async.RunSynchronously
    configuration

let private generateExtraConsumer (worker: SceneStateWorker) (token: CancellationToken) =
    async {
        while not token.IsCancellationRequested do
            let! newSceneSequence = worker.GetNextNewExtraScene()
            newSceneSequence |> AsyncSeq.toListSynchronously |> ignore // TODO Process newSceneSequence
        return ()
    } |> Async.Start

let private subscribe (worker: SceneStateWorker) (hubConnection: HubConnection) =
    subscribeOnScene hubConnection (fun (action, synchronizer) ->
        worker.SendNewMessage {
            Action = action
            Synchronizer = synchronizer
        }
    )
    hubConnection

let private initializeHubConnection (configuration: ApiConfiguration) =
    let worker = SceneStateWorker.Unit()
    let tokenSource = new CancellationTokenSource()
    generateExtraConsumer worker tokenSource.Token
    let connection =
        openConnection tokenSource (sprintf "%s/%s" configuration.Host configuration.HubPath)
        |> subscribe worker 
    (worker, tokenSource, connection)

let setupGameConnection (configuration: RawConfigurationWithStorageConnection) =
    let worker, tokenSource, hub =
        configuration.Api
        |> authorize
        |> initializeHubConnection
    {
        Learning = configuration.Learning
        ApiHost = configuration.Api.Host
        Hub = hub
        Storage = configuration.Storage
        Worker = worker
        WorkerCancellationToken = tokenSource.Token
    }