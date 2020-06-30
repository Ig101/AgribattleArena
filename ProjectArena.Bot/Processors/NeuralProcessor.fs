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
open ProjectArena.Bot.Helpers.Strategies.AggressiveHelper
open ProjectArena.Bot.Helpers.Strategies.DefenciveHelper
open ProjectArena.Bot.Helpers.Strategies.FleeHelper

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

let private getNeuralNetworkOutputs (random: Random) (configuration: Configuration) (neurons: Neuron list) (network: NeuralModel) =
    network.Layers |> Seq.toList |> List.sortBy (fun l -> l.SortIndex) |> List.fold (fun n l -> l.Outputs |> Seq.toList |> List.map (getOutputNeuron random configuration n)) neurons

let private calculateNeededAction (configuration: Configuration) (model: NeuralModelContainer, scene: Scene) (actor: ActorDto) = async {
    do! Task.Delay(200)
    let random = Random()
    let! workingModel = model.Unpack()
    let strategy, _ =
        workingModel
        |> getNeuralNetworkOutputs random configuration (getInputNeurons (Some scene))
        |> Seq.toList
        |> List.map (convertNeuronToStrategy)
        |> List.sortByDescending (fun (_, v) -> v)
        |> List.head

    let action = match strategy with
                 | Aggressive -> getAggressiveAction (scene, actor)
                 | Defencive -> getDefenciveAction (scene, actor)
                 | Flee ->  getFleeAction (scene, actor)
    return (action, actor)
}

let private orderAction (configuration: Configuration) (sceneId: Guid) (command: SceneAction, actor: ActorDto) =
    GC.Collect()
    match command with
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