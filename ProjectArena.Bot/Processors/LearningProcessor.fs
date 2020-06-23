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
open Microsoft.Extensions.Logging
open MongoDB.Driver
open ProjectArena.Bot.Functors

let private getModelsFromStorage (configuration:Configuration) =
    let context = BotContext(configuration.Storage);
    context.NeuralModelDefinitions.GetAsync (fun m -> m.Key)
    |> Async.AwaitTask
    |> Async.map (fun vSeq -> vSeq |> Seq.map(fun v -> NeuralModelContainer.Pack configuration v.Id) |> Seq.toList)

let private refillTableWithNewModels (configuration:Configuration) (models: NeuralModelContainer seq) =
    let context = BotContext(configuration.Storage)
    let modelIds = models |> Seq.map(fun v -> v.GetId()) |> Seq.toList
    context.NeuralModels.Delete(Builders<NeuralModel>.Filter.In ((fun m -> m.Id), modelIds) |> Builders<NeuralModel>.Filter.Not)
    context.NeuralModelDefinitions.Delete( Builders<NeuralModelDefinition>.Filter.In ((fun m -> m.Id), modelIds) |> Builders<NeuralModelDefinition>.Filter.Not)
    context.NeuralModelDefinitions.Update((fun _ -> true), Builders<NeuralModelDefinition>.Update.Set((fun x -> x.Key), true))
    context.ApplyChangesAsync() |> Async.AwaitTask

let startLearning (configuration:Configuration) =
    async {
        while not configuration.WorkerCancellationToken.IsCancellationRequested do
            configuration.Logger.LogInformation "Learning cycle started."
            do!
                getModelsFromStorage configuration
                |> Async.bind (breed configuration)
                |> Async.bind (processSequenceAsynchronously configuration.Learning.BatchSize (processScene configuration))
                |> Async.map (select configuration)
                |> Async.bind (refillTableWithNewModels configuration)
            configuration.Logger.LogInformation "Learning cycle finished."
        configuration.Logger.LogError "Learning aborted due to unexpected error."
    } |> Async.Start
    configuration