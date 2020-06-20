module ProjectArena.Bot.Processors.NeuralProcessor
open System
open ProjectArena.Bot.Domain.BotMongoContext.Entities

let initializeRandomNeuralModel() =
    // TODO Setup neural models with random
    {
        NeuralModel.Id = Guid.NewGuid().ToString()
    }