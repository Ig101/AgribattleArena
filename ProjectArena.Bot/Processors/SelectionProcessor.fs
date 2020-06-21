module ProjectArena.Bot.Processors.SelectionProcessor
open ProjectArena.Bot.Domain.BotMongoContext.Entities
open ProjectArena.Bot.Models.Configuration

let select (configuration: Configuration) (modelsWithPerformance: (NeuralModel * float) seq): NeuralModel seq =
    modelsWithPerformance |> Seq.map(fun (m, k) -> m)