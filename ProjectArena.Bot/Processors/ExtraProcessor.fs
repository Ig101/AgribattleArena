module ProjectArena.Bot.Processors.ExtraProcessor
open ProjectArena.Bot.Models.Configuration
open ProjectArena.Bot.Processors.SceneCreationProcessor
open ProjectArena.Infrastructure.Mongo
open FSharp.Control
open ProjectArena.Bot.Models.States

let enrichSceneSequenceWithNeuralModels (connection: IMongoConnection) (sequence: AsyncSeq<IncomingSynchronizationMessage>) = async {
    let! neuralModel = getRandomNeuralModel connection
    return ((neuralModel, neuralModel), sequence)
}


let startExtraProcessing (configuration: Configuration) =
    async {
        while not configuration.WorkerCancellationToken.IsCancellationRequested do
            printfn "Waiting for new extra scene."
            let! newSceneSequence = configuration.Worker.GetNextNewExtraScene()
            newSceneSequence
            |> enrichSceneSequenceWithNeuralModels configuration.Storage
            |> Async.bind (fun r -> r ||> processCreatedSceneSequence configuration)
            |> Async.Ignore
            |> Async.Start
            printfn "Extra scene found"
        return ()
    } |> Async.Start
    configuration