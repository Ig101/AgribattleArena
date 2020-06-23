module ProjectArena.Bot.Processors.SelectionProcessor
open ProjectArena.Bot.Domain.BotMongoContext.Entities
open ProjectArena.Bot.Models.Configuration
open ProjectArena.Bot.Functors
open Microsoft.Extensions.Logging

let select (configuration: Configuration) (modelIdsWithPerformance: (NeuralModelContainer * float) seq): NeuralModelContainer seq =
    let _, maxScore = modelIdsWithPerformance |> Seq.maxBy (fun (_, v) -> v)
    let averageScore = modelIdsWithPerformance |> Seq.averageBy (fun (_, v) -> v)
    configuration.Logger.LogInformation (sprintf "Selection... Max performance: %f. Average performance: %f" maxScore averageScore)
    modelIdsWithPerformance
    |> Seq.sortByDescending (fun (_, key) -> key)
    |> Seq.take (configuration.Learning.SuccessfulModelsAmount)
    |> Seq.map(fun (m, k) -> m)