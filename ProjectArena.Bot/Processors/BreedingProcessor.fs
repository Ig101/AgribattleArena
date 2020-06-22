module ProjectArena.Bot.Processors.BreedingProcessor
open ProjectArena.Bot.Domain.BotMongoContext.Entities
open ProjectArena.Bot.Models.Configuration
open System

let breed (configuration: Configuration) (models: NeuralModel seq): NeuralModel seq =
    models |> Seq.map (fun _ -> { Id = Guid.NewGuid().ToString() })