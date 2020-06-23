module ProjectArena.Bot.Processors.BreedingProcessor
open ProjectArena.Bot.Domain.BotMongoContext.Entities
open ProjectArena.Bot.Models.Configuration
open System
open ProjectArena.Bot.Functors
open FSharp.Control
open ProjectArena.Bot.Domain.BotMongoContext.EntityModels
open ProjectArena.Bot.Domain.BotMongoContext

let private commitNewModel (configuration: Configuration) (model: NeuralModel) = async {
    let context = BotContext(configuration.Storage)
    context.NeuralModels.InsertOne(model)
    context.NeuralModelDefinitions.InsertOne({
        Id = model.Id
        Key = false
    })
    do! context.ApplyChangesAsync()
    return NeuralModelContainer.Pack configuration model.Id
}

let private processInput (configuration: Configuration) (random: Random) (father: NeuralOutputGroup) (input: NeuralBond) =
    let neededValue =
        match random.NextDouble() with
        | v when v < configuration.Learning.MutationProbability -> random.NextDouble()
        | _ -> match random.NextDouble() with
               | v when v < 0.5 -> (father.Inputs |> Seq.find (fun i -> i.Input = input.Input)).Weight
               | _ -> input.Weight
    {
        Input = input.Input
        Weight = neededValue
    }

let private processOutput (configuration: Configuration) (random: Random) (father: NeuralLayer) (output: NeuralOutputGroup) =
    let fatherOutput = father.Outputs |> Seq.find (fun o -> o.Output = output.Output)
    {
        Output = output.Output
        Inputs = output.Inputs |> Seq.map (processInput configuration random fatherOutput)
    }

let private processLayer (configuration: Configuration) (random: Random) (father: NeuralNetwork) (layer: NeuralLayer) =
    let fatherLayer = father.Layers |> Seq.find (fun l -> l.SortIndex = layer.SortIndex)
    {
        SortIndex = layer.SortIndex
        Outputs = layer.Outputs |> Seq.map (processOutput configuration random fatherLayer)
    }

let private crossingoverAndMutation (configuration: Configuration) (random: Random) (father: NeuralModel, mother: NeuralModel) =
    {
        Id = Guid.NewGuid().ToString()
        MagnifyingNetwork = {
            Layers = mother.MagnifyingNetwork.Layers |> Seq.map (processLayer configuration random father.MagnifyingNetwork)
        }
        CommandNetwork = {
            Layers = mother.CommandNetwork.Layers |> Seq.map (processLayer configuration random father.CommandNetwork)
        }
    }

let private breedOne (configuration: Configuration) (random: Random) (models: NeuralModelContainer list) = 
    async {
        let firstIndex = random.Next(models |> Seq.length);
        let secondIndexRaw = random.Next((models |> Seq.length) - 1)
        let secondIndex = match secondIndexRaw with
                          | index when index >= firstIndex -> index + 1
                          | index -> index
        let! father = models.[firstIndex].Unpack()
        let! mother = models.[secondIndex].Unpack()
        return (father, mother)
        |> crossingoverAndMutation (configuration: Configuration) (random: Random)
    } |> Async.bind (commitNewModel configuration)

let breed (configuration: Configuration) (models: NeuralModelContainer seq) =
    let random = Random()
    [1..configuration.Learning.ModelsAmount]
        |> AsyncSeq.ofSeq
        |> AsyncSeq.mapAsync (fun _ -> async {
                let! newModel = breedOne configuration random (models |> Seq.toList)
                GC.Collect()
                return newModel
            })
        |> AsyncSeq.toListAsync