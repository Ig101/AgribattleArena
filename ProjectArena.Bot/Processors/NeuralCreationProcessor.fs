module ProjectArena.Bot.Processors.NeuralCreationProcessor
open System
open ProjectArena.Bot.Domain.BotMongoContext.Entities
open ProjectArena.Bot.Domain.BotMongoContext.EntityModels
open ProjectArena.Bot.Models.Configuration
open ProjectArena.Bot.Helpers.Neural.NeuralHelper
open ProjectArena.Bot.Helpers.Neural.OutputHelper

let private initializeLayer (random: Random) inputs outputs =
    outputs
    |> List.map (fun name -> {
        Output = name
        Shift = random.NextDouble()
        Inputs = inputs |> List.map (fun input -> { Input = input; Weight = random.NextDouble() })
    })

let private initializeHiddenLayer (random: Random) (configuration: LearningConfiguration) =
    getHiddenNeuronNames 'h' configuration.HiddenNeuronsCount
    |> initializeLayer random (getInputNeuronNames())

let private initializeOutputLayer (random: Random) (configuration: LearningConfiguration) =
    getOutputNeuronNames()
    |> initializeLayer random (getHiddenNeuronNames 'h' configuration.HiddenNeuronsCount)

let initializeRandomNeuralModel (random: Random) (configuration: LearningConfiguration) =
    {
        NeuralModel.Id = Guid.NewGuid().ToString()
        Layers = [
            {
                SortIndex = 1y
                Outputs = initializeHiddenLayer random configuration
            }
            {
                SortIndex = 3y
                Outputs = initializeOutputLayer random configuration
            }
        ]
    }