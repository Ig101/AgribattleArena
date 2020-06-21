module ProjectArena.Bot.Processors.ExtraProcessor
open ProjectArena.Bot.Models.Configuration
open ProjectArena.Bot.Processors.SceneCreationProcessor

let startExtraProcessing (configuration: Configuration) =
    async {
        while not configuration.WorkerCancellationToken.IsCancellationRequested do
            printfn "Waiting for new extra scene."
            let! newSceneSequence = configuration.Worker.GetNextNewExtraScene()
            let! neuralModel = getRandomNeuralModel configuration.Storage
            printfn "Extra scene found. Model id: %s." neuralModel.Id
            do!
                newSceneSequence
                |> processCreatedSceneSequence configuration (neuralModel, neuralModel)
                |> Async.Ignore
        return ()
    } |> Async.Start
    configuration