module ProjectArena.Bot.Processors.SceneCreationProcessor
open ProjectArena.Bot.Domain.BotMongoContext.Entities
open ProjectArena.Bot.Models.Configuration
open FSharp.Control
open ProjectArena.Bot.Models.States
open ProjectArena.Bot.Domain.GameConnection.GameApi
open ProjectArena.Infrastructure.Mongo
open ProjectArena.Bot.Domain.BotMongoContext
open ProjectArena.Bot.Processors.SceneProcessor
open Microsoft.Extensions.Logging
open ProjectArena.Bot.Functors

let processCreatedSceneSequence (configuration: Configuration) (model: NeuralModelContainer, spareModel: NeuralModelContainer) (sequence: AsyncSeq<IncomingSynchronizationMessage>) = async {
    let! performance =
        sequence
        |> AsyncSeq.foldAsync (sceneMessageProcessor configuration (model, spareModel)) None
        |> Async.map (tryCalculatePerformance configuration)
    return (model, performance)
}

let getRandomNeuralModel (configuration: Configuration) =
    let context = BotContext configuration.Storage
    context.NeuralModelDefinitions.GetRandomOneAsync(fun m -> m.Key)
    |> Async.AwaitTask
    |> Async.map (fun result -> NeuralModelContainer.Pack configuration result.Id)

let processScene (configuration: Configuration) (model: NeuralModelContainer) = async {
    let! spareNeuralModel = getRandomNeuralModel configuration
    if (configuration.Learning.IsLearning) then
        configuration.Logger.LogInformation "Enqueueing..."
        do! enqueue configuration.Logger configuration.User.AuthCookie configuration.ApiHost
    configuration.Logger.LogInformation (sprintf "Waiting for new scene for learning. Model id: %s" (model.GetId()))
    let! newSceneSequence = configuration.Worker.GetNextNewScene()
    let! result = processCreatedSceneSequence configuration (model, spareNeuralModel) newSceneSequence
    return result
} 