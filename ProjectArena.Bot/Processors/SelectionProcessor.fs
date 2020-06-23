module ProjectArena.Bot.Processors.SelectionProcessor
open ProjectArena.Bot.Domain.BotMongoContext.Entities
open ProjectArena.Bot.Models.Configuration
open ProjectArena.Bot.Functors

let select (configuration: Configuration) (modelIdsWithPerformance: (NeuralModelContainer * float) seq): NeuralModelContainer seq =
    modelIdsWithPerformance |> Seq.map(fun (m, k) -> m)