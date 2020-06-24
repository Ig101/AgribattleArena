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
open ProjectArena.Bot.Helpers.SceneHelper
open ProjectArena.Bot.Helpers.ActionsHelper
open System.Diagnostics

let private calculateNeuronValue (random: Random) (neurons: Neuron list) (bond: NeuralBond) =
    let neuronValue = neurons |> List.tryFind (fun n -> n.Name = bond.Input) |> Option.map (fun n -> n.Value)
    match neuronValue with
    | Some value -> value * bond.Weight
    | None -> random.NextDouble() * bond.Weight

let private getOutputNeuron (random: Random) (configuration: Configuration) (neurons: Neuron list) (weights: NeuralOutputGroup) =
    let value = ((weights.Inputs |> Seq.toList |> List.sumBy (calculateNeuronValue random neurons)) + weights.Shift) / configuration.Learning.ActivationDivider
    {
        Name = weights.Output
        Value = normalize value
    }

let private getNeuralNetworkOutputs (random: Random) (configuration: Configuration) (neurons: Neuron list) (network: NeuralNetwork) =
    network.Layers |> Seq.toList |> List.sortBy (fun l -> l.SortIndex) |> List.fold (fun n l -> l.Outputs |> Seq.toList |> List.map (getOutputNeuron random configuration n)) neurons

let private calculateNeededAction (configuration: Configuration) (model: NeuralModelContainer, scene: Scene) (actor: ActorDto) = async {
    let player = scene.Players |> Seq.find (fun p -> p.Id = actor.OwnerId.Value)
    let random = Random()
    let! workingModel = model.Unpack()
    let magnifyingResult, _ =
        workingModel.MagnifyingNetwork
        |> getNeuralNetworkOutputs random configuration (getMagnifyingInputNeurons (Some scene))
        |> Seq.toList
        |> List.map (convertMagnifyingNeuronToAction)
        |> List.sortByDescending (fun (_, v) -> v)
        |> List.find (fun (a, _) -> match a with | Wait -> true | Proceed (x, y) -> isAnyActionAllowedOnBlock scene (actor, player) (x, y))

    match magnifyingResult with
    | Wait -> return (ActionNeuronType.Wait, actor)
    | Proceed (x, y) ->
        let shift = (x, y)
        let commandResult, _ =
            workingModel.CommandNetwork
            |> getNeuralNetworkOutputs random configuration (getCommandInputNeurons shift (Some scene))
            |> Seq.toList
            |> List.map (convertCommandNeuronToAction shift)
            |> List.sortByDescending (fun (_, v) -> v)
            |> List.find (fun (a, _) -> isActionAllowed scene (actor, player) a)
        return (commandResult, actor)
}

let private orderAction (configuration: Configuration) (sceneId: Guid) (command: ActionNeuronType, actor: ActorDto) =
    GC.Collect()
    match command with
    | ActionNeuronType.Wait ->
        orderWait configuration.Hub (sceneId, actor.Id)
    | Move (x, y) ->
        orderMove configuration.Hub (sceneId, actor.Id, x, y)
    | Cast (name, x, y) ->
        match actor.AttackingSkill |> Option.filter (fun s -> s.NativeId = name) with
        | Some s -> orderAttack configuration.Hub (sceneId, actor.Id, x, y)
        | None ->
            let skill = actor.Skills |> Seq.find (fun s -> s.NativeId = name)
            orderCast configuration.Hub (sceneId, actor.Id, skill.Id, x, y)

let actOnScene (configuration: Configuration) (model: NeuralModelContainer, scene: Scene) =
    match scene.TempActor with
    | Some actor ->
        actor
        |> calculateNeededAction configuration (model, scene)
        |> Async.map (orderAction configuration scene.Id)
    | None ->
        Async.result ()