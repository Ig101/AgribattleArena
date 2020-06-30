module ProjectArena.Bot.Helpers.Neural.NeuralHelper
open ProjectArena.Bot.Models.States
open ProjectArena.Bot.Models.Neural
open ProjectArena.Bot.Helpers.Neural.CellsHelper
open ProjectArena.Bot.Helpers.Neural.SelfHelper
open ProjectArena.Bot.Helpers.Neural.OutputHelper
open System.Diagnostics

let getInputNeurons sceneOpt  =
    getCellsInputNeurons sceneOpt
    |> List.append (getSelfNeuron sceneOpt)
    |> List.append (getSelfSkillsNeurons sceneOpt)

let getInputNeuronNames ()  =
    getCellsInputNeurons None |> List.map (fun n -> n.Name)
    |> List.append (getSelfNeuron None |> List.map (fun n -> n.Name))
    |> List.append (getSelfSkillsNeurons None |> List.map (fun n -> n.Name))

let getHiddenNeuronNames (identifier: char) (amount: int) =
    [1..amount]
    |> List.map (fun digit -> sprintf "%s%i" (identifier.ToString()) digit)