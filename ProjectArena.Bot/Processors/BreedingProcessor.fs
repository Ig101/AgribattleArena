module ProjectArena.Bot.Processors.BreedingProcessor
open ProjectArena.Bot.Domain.BotMongoContext.Entities
open ProjectArena.Bot.Models.Configuration
open System
open ProjectArena.Bot.Functors

let breed (configuration: Configuration) (modelIds: NeuralModelContainer seq): NeuralModelContainer seq =
    modelIds