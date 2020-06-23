module ProjectArena.Bot.Helpers.Neural.NeuralHelper
open ProjectArena.Bot.Models.States
open ProjectArena.Bot.Models.Neural
open ProjectArena.Bot.Helpers.Neural.CellsHelper
open ProjectArena.Bot.Helpers.Neural.KeyHelper
open ProjectArena.Bot.Helpers.Neural.SelfHelper
open ProjectArena.Bot.Helpers.Neural.OutputHelper

let getMagnifyingInputNeurons sceneAndActorIdOpt  =
    getMagnifyingCellsInputNeurons sceneAndActorIdOpt |> List.map (fun n -> n.Name)
    |> List.append (getMagnifyingKeyInputNeurons sceneAndActorIdOpt |> List.map (fun n -> n.Name))
    |> List.append (getMagnifyingSelfNeuron sceneAndActorIdOpt |> List.map (fun n -> n.Name))

let getMagnifyingInputNeuronNames ()  =
    getMagnifyingCellsInputNeurons None |> List.map (fun n -> n.Name)
    |> List.append (getMagnifyingKeyInputNeurons None |> List.map (fun n -> n.Name))
    |> List.append (getMagnifyingSelfNeuron None |> List.map (fun n -> n.Name))

let getCommandInputNeurons shift sceneAndActorIdOpt  =
    getCommandCellsInputNeurons shift sceneAndActorIdOpt |> List.map (fun n -> n.Name)
    |> List.append (getKeyInputNeurons shift sceneAndActorIdOpt |> List.map (fun n -> n.Name))
    |> List.append (getSelfNeuron shift sceneAndActorIdOpt |> List.map (fun n -> n.Name))

let getCommandInputNeuronNames () =
    getCommandCellsInputNeurons (0, 0) None |> List.map (fun n -> n.Name)
    |> List.append (getKeyInputNeurons (0, 0) None |> List.map (fun n -> n.Name))
    |> List.append (getSelfNeuron (0, 0) None |> List.map (fun n -> n.Name))

let getHiddenNeuronNames (identifier: char) (amount: int) =
    [1..amount]
    |> List.map (fun digit -> sprintf "%s%i" (identifier.ToString()) digit)