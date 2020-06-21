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
open ProjectArena.Infrastructure.Mongo

let private getModelsFromStorage (connection: IMongoConnection) =
    let context = BotContext(connection);
    context.NeuralModels.GetAsync (fun _ -> true) |> Async.AwaitTask

let private refillTableWithNewModels (connection: IMongoConnection) (models: NeuralModel seq) =
    let context = BotContext(connection);
    context.NeuralModels.Delete(fun _ -> true)
    context.NeuralModels.Insert(models)
    context.ApplyChangesAsync() |> Async.AwaitTask

let startLearning (configuration:Configuration) =
    async {
        while not configuration.WorkerCancellationToken.IsCancellationRequested do
            printfn "Learning cycle started."
            do!
                getModelsFromStorage configuration.Storage
                |> Async.map (breed configuration)
                |> Async.bind (processSequenceAsynchronously (processScene configuration))
                |> Async.map (select configuration)
                |> Async.bind (refillTableWithNewModels configuration.Storage)
            printfn "Learning cycle finished."
        printfn "Learning aborted due to unexpected error."
    } |> Async.Start
    configuration