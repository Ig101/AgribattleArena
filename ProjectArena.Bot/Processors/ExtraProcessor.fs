module ProjectArena.Bot.Processors.ExtraProcessor
open ProjectArena.Bot.Models.Configuration
open ProjectArena.Bot.Processors.SceneCreationProcessor
open ProjectArena.Infrastructure.Mongo
open FSharp.Control
open ProjectArena.Bot.Models.States
open Microsoft.Extensions.Logging

let enrichSceneSequenceWithNeuralModels (configuration: Configuration) (sequence: AsyncSeq<IncomingSynchronizationMessage>) = async {
    let! neuralModel = getRandomNeuralModel configuration
    return ((neuralModel, neuralModel), sequence)
}


let startExtraProcessing (configuration: Configuration) =
    async {
        while not configuration.WorkerCancellationToken.IsCancellationRequested do
            configuration.Logger.LogInformation "Waiting for new extra scene."
            let! newSceneSequence = configuration.Worker.GetNextNewExtraScene()
            newSceneSequence
            |> enrichSceneSequenceWithNeuralModels configuration
            |> Async.bind (fun r -> r ||> processCreatedSceneSequence configuration)
            |> Async.Ignore
            |> Async.Start
        return ()
    } |> Async.Start
    configuration