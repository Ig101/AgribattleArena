module ProjectArena.Bot.Processors.SelectionProcessor
open ProjectArena.Bot.Domain.BotMongoContext.Entities
open ProjectArena.Bot.Models.Configuration
open ProjectArena.Bot.Functors
open Microsoft.Extensions.Logging

let select (configuration: Configuration) (modelsWithPerformance: (NeuralModelContainer * float) seq): NeuralModelContainer seq =
    let _, maxScore = modelsWithPerformance |> Seq.maxBy (fun (_, v) -> v)
    let averageScore = modelsWithPerformance |> Seq.averageBy (fun (_, v) -> v)
    let modelsList = modelsWithPerformance |> Seq.map (fun (m, p) -> (m.GetId(), p)) |> Seq.toList
    configuration.Logger.LogInformation (sprintf "Selection... Max performance: %f. Average performance: %f. All models: %A" maxScore averageScore modelsList)
    modelsWithPerformance
    |> Seq.sortByDescending (fun (_, key) -> key)
    |> Seq.take (configuration.Learning.SuccessfulModelsAmount)
    |> Seq.map(fun (m, k) -> m)