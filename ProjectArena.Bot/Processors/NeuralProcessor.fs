module ProjectArena.Bot.Processors.NeuralProcessor
open System
open ProjectArena.Bot.Domain.BotMongoContext.Entities
open ProjectArena.Bot.Domain.BotMongoContext.EntityModels
open ProjectArena.Bot.Models.States
open Microsoft.AspNetCore.SignalR.Client
open ProjectArena.Bot.Domain.GameConnection.SignalRConnection
open ProjectArena.Bot.Models.Configuration
open ProjectArena.Bot.Helpers.Neural.NeuralHelper
open ProjectArena.Bot.Helpers.Neural.OutputHelper
open ProjectArena.Bot.Domain.BotMongoContext
open ProjectArena.Bot.Functors
open System.Threading.Tasks
open ProjectArena.Bot.Models.Neural
open ProjectArena.Bot.Models.Dtos

let private calculateNeuronValue (random: Random) (neurons: Neuron seq) (bond: NeuralBond) =
    let neuronValue = neurons |> Seq.tryFind (fun n -> n.Name = bond.Input) |> Option.map (fun n -> n.Value)
    match neuronValue with
    | Some value -> value * bond.Weight
    | None -> random.NextDouble() * bond.Weight

let private getOutputNeuron (random: Random) (configuration: Configuration) (neurons: Neuron seq) (weights: NeuralOutputGroup) =
    let value = ((weights.Inputs |> Seq.sumBy (calculateNeuronValue random neurons)) + weights.Shift) / configuration.Learning.ActivationDivider
    {
        Name = weights.Output
        Value = 1.0 / (1.0 + Math.Pow (Math.E, -value))
    }

let private getNeuralNetworkOutputs (random: Random) (configuration: Configuration) (neurons: Neuron seq) (network: NeuralNetwork) =
    network.Layers |> Seq.sortBy (fun l -> l.SortIndex) |> Seq.fold (fun n l -> l.Outputs |> Seq.map (getOutputNeuron random configuration n)) neurons

let private calculateNeededAction (configuration: Configuration) (model: NeuralModelContainer, scene: Scene) (actor: ActorDto) = async {
    let random = Random()
    let! workingModel = model.Unpack()
    let magnifyingResult =
        workingModel.MagnifyingNetwork
        |> getNeuralNetworkOutputs random configuration (getMagnifyingInputNeurons (Some scene))
        |> Seq.map (convertMagnifyingNeuronToAction)
    // TODO ProcessResultsAndGetShift
    let chosenSector = magnifyingResult |> Seq.head
    match chosenSector with
    | Wait -> return (ActionNeuronType.Wait, actor.Id)
    | Proceed (x, y) ->
        let shift = (x, y)
        let commandResult =
            workingModel.CommandNetwork
            |> getNeuralNetworkOutputs random configuration (getCommandInputNeurons shift (Some scene))
            |> Seq.map (convertCommandNeuronToAction)
        // TODO GetSuitableBestAction
        return ((commandResult |> Seq.head), actor.Id)
}

let private orderAction (configuration: Configuration) (sceneId: Guid) (command: ActionNeuronType, actorId: int) =
    GC.Collect()
    orderWait configuration.Hub (sceneId, actorId)
    ()

let actOnScene (configuration: Configuration) (model: NeuralModelContainer, scene: Scene) =
    match scene.TempActor with
    | Some actor ->
        actor
        |> calculateNeededAction configuration (model, scene)
        |> Async.map (orderAction configuration scene.Id)
    | None ->
        Async.result ()