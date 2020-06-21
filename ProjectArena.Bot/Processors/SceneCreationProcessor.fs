module ProjectArena.Bot.Processors.SceneCreationProcessor
open ProjectArena.Bot.Domain.BotMongoContext.Entities
open ProjectArena.Bot.Models.Configuration
open FSharp.Control
open ProjectArena.Bot.Models.States
open ProjectArena.Bot.Domain.GameConnection.GameApi
open ProjectArena.Infrastructure.Mongo
open ProjectArena.Bot.Domain.BotMongoContext
open ProjectArena.Bot.Processors.SceneProcessor

let processCreatedSceneSequence (configuration: Configuration) (model: NeuralModel, spareModel: NeuralModel) (sequence: AsyncSeq<IncomingSynchronizationMessage>) = async {
    let! performance =
        sequence
        |> AsyncSeq.foldAsync (sceneMessageProcessor configuration (model, spareModel)) None
        |> Async.map tryCalculatePerformance
    return (model, performance)
}

let getRandomNeuralModel (connection: IMongoConnection) =
    let context = BotContext connection
    context.NeuralModels.GetRandomOneAsync(fun _ -> true)
    |> Async.AwaitTask

let processScene (configuration: Configuration) (model: NeuralModel) = async {
    let! spareNeuralModel = getRandomNeuralModel configuration.Storage
    if (configuration.Learning.IsLearning) then
        printfn "Enqueueing..."
        do! enqueue configuration.User.AuthCookie configuration.ApiHost
        printfn "Enqueue completed"
    printfn "Waiting for new scene for learning. Model id: %s" model.Id
    let! newSceneSequence = configuration.Worker.GetNextNewScene()
    printfn "Scene found. Model id: %s." model.Id
    let! result = processCreatedSceneSequence configuration (model, spareNeuralModel) newSceneSequence
    return result
} 