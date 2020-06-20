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
open ProjectArena.Bot.Domain.BotMongoContext
open ProjectArena.Bot.Processors.SceneCreationProcessor

let private authorize (configuration: ApiConfiguration) =
    printfn "Authorizing..."
    configuration.Host
    |> authorize configuration.Login configuration.Password
    |> Async.RunSynchronously
    configuration

let private generateExtraConsumer (storageConnection: IMongoConnection) (worker: SceneStateWorker) (token: CancellationToken) =
    async {
        while not token.IsCancellationRequested do
            printfn "Waiting for new extra scene."
            let! newSceneSequence = worker.GetNextNewExtraScene()
            let! neuralModel = getRandomNeuralModel storageConnection
            printfn "Extra scene found. Model id: %s." neuralModel.Id
            do!
                newSceneSequence
                |> processCreatedSceneSequence (neuralModel, neuralModel)
                |> Async.Ignore
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

let private initializeWorker (storageConnection: IMongoConnection) (configuration: ApiConfiguration) =
    printfn "Loading Worker..."
    let worker = SceneStateWorker.Unit()
    let tokenSource = new CancellationTokenSource()
    generateExtraConsumer storageConnection worker tokenSource.Token
    (worker, tokenSource, configuration)

let private initializeHubConnection (worker: SceneStateWorker, tokenSource: CancellationTokenSource, configuration: ApiConfiguration) =
    printfn "Loading Connection..."
    let connection =
        openConnection tokenSource (sprintf "%s/%s" configuration.Host configuration.HubPath)
        |> subscribe worker 
    (worker, tokenSource, connection)

let setupGameConnection (configuration: RawConfigurationWithStorageConnection) =
    let worker, tokenSource, hub =
        configuration.Api
        |> authorize
        |> initializeWorker configuration.Storage
        |> initializeHubConnection
    printfn "Loading finished."
    {
        Learning = configuration.Learning
        ApiHost = configuration.Api.Host
        Hub = hub
        Storage = configuration.Storage
        Worker = worker
        WorkerCancellationToken = tokenSource.Token
    }

let dispose (configuration: Configuration) =
    configuration.Hub.DisposeAsync().Wait()
    ()
