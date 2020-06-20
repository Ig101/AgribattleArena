module ProjectArena.Bot.Processors.LearningProcessor
open ProjectArena.Bot.Models.Configuration
open ProjectArena.Bot.Domain.BotMongoContext
open ProjectArena.Bot.Processors.BreedingProcessor
open ProjectArena.Bot.Processors.SelectionProcessor
open FSharp.Control
open FSharp.Collections.ParallelSeq
open ProjectArena.Bot.Domain.BotMongoContext.Entities
open ProjectArena.Bot.Processors.SceneCreationProcessor
open System.Threading.Tasks
open ProjectArena.Bot.Helpers.AsyncHelper

let private getModelsFromStorage (context: BotContext) =
    context.NeuralModels.GetAsync (fun _ -> true) |> Async.AwaitTask

let private refillTableWithNewModels (context: BotContext) (models: NeuralModel seq) =
    context.NeuralModels.Delete(fun _ -> true)
    context.NeuralModels.Insert(models)
    context.ApplyChangesAsync() |> Async.AwaitTask

let startLearning (configuration:Configuration) =
    async {
        while not configuration.WorkerCancellationToken.IsCancellationRequested do
            printfn "Learning cycle started."
            let context = BotContext(configuration.Storage);
            do!
                getModelsFromStorage context
                |> Async.map (breed configuration)
                |> Async.bind (processSequenceAsynchronously (processScene configuration))
                |> Async.map (select configuration)
                |> Async.bind (refillTableWithNewModels context)
            printfn "Learning cycle finished."
        printfn "Learning aborted due to unexpected error."
    } |> Async.Start