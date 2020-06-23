module ProjectArena.Bot.Helpers.Neural.NeuralHelper
open ProjectArena.Bot.Models.States
open ProjectArena.Bot.Models.Neural
open ProjectArena.Bot.Helpers.Neural.CellsHelper
open ProjectArena.Bot.Helpers.Neural.KeyHelper
open ProjectArena.Bot.Helpers.Neural.SelfHelper
open ProjectArena.Bot.Helpers.Neural.OutputHelper

let getMagnifyingInputNeurons sceneOpt  =
    getMagnifyingCellsInputNeurons sceneOpt
    |> List.append (getMagnifyingKeyInputNeurons sceneOpt)
    |> List.append (getMagnifyingSelfNeuron sceneOpt)

let getMagnifyingInputNeuronNames ()  =
    getMagnifyingCellsInputNeurons None |> List.map (fun n -> n.Name)
    |> List.append (getMagnifyingKeyInputNeurons None |> List.map (fun n -> n.Name))
    |> List.append (getMagnifyingSelfNeuron None |> List.map (fun n -> n.Name))

let getCommandInputNeurons shift sceneOpt  =
    getCommandCellsInputNeurons shift sceneOpt
    |> List.append (getKeyInputNeurons shift sceneOpt)
    |> List.append (getSelfNeuron shift sceneOpt)

let getCommandInputNeuronNames () =
    getCommandCellsInputNeurons (0, 0) None |> List.map (fun n -> n.Name)
    |> List.append (getKeyInputNeurons (0, 0) None |> List.map (fun n -> n.Name))
    |> List.append (getSelfNeuron (0, 0) None |> List.map (fun n -> n.Name))

let getHiddenNeuronNames (identifier: char) (amount: int) =
    [1..amount]
    |> List.map (fun digit -> sprintf "%s%i" (identifier.ToString()) digit)