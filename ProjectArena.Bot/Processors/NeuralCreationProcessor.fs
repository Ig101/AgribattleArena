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
        Inputs = inputs |> List.map (fun input -> { Input = input; Weight = random.NextDouble() })
    })

let private initializeHiddenMagnifyingLayer (random: Random) (configuration: LearningConfiguration) =
    getHiddenNeuronNames 'h' configuration.MagnifyingHiddenNeuronsCount
    |> initializeLayer random (getMagnifyingInputNeuronNames())

let private initializeOutputMagnifyingLayer (random: Random) (configuration: LearningConfiguration) =
    getMagnifyingOutputNeuronNames()
    |> initializeLayer random (getHiddenNeuronNames 'h' configuration.MagnifyingHiddenNeuronsCount)

let private initializeHiddenCommandLayer (random: Random) (configuration: LearningConfiguration) =
    getHiddenNeuronNames 'h' configuration.CommandHiddenNeuronsCount
    |> initializeLayer random (getCommandInputNeuronNames())

let private initializeOutputCommandLayer (random: Random) (configuration: LearningConfiguration) =
    getCommandOutputNeuronNames()
    |> initializeLayer random (getHiddenNeuronNames 'h' configuration.CommandHiddenNeuronsCount)

let initializeRandomNeuralModel (random: Random) (configuration: LearningConfiguration) =
    // TODO Setup neural models with random
    {
        NeuralModel.Id = Guid.NewGuid().ToString()
        MagnifyingNetwork = {
            Layers = [
                {
                    SortIndex = 1y
                    Outputs = initializeHiddenMagnifyingLayer random configuration
                }
                {
                    SortIndex = 3y
                    Outputs = initializeOutputMagnifyingLayer random configuration
                }
            ]
        }
        CommandNetwork = {
            Layers = [
                {
                    SortIndex = 1y
                    Outputs = initializeHiddenCommandLayer random configuration
                }
                {
                    SortIndex = 3y
                    Outputs = initializeOutputCommandLayer random configuration
                }
            ]
        }
    }