module ProjectArena.Bot.Processors.BreedingProcessor
open ProjectArena.Bot.Domain.BotMongoContext.Entities
open ProjectArena.Bot.Models.Configuration

let breed (configuration: Configuration) (models: NeuralModel seq): NeuralModel seq =
    models