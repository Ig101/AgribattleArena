module ProjectArena.Bot.Helpers.Neural.OutputHelper
open System
open ProjectArena.Bot.Models.Neural

let convertNeuronToStrategy (neuron: Neuron) =
    let typeSymbol = neuron.Name
    match typeSymbol with
    | "a" -> (Aggressive, neuron.Value)
    | "d" -> (Defencive, neuron.Value)
    | "f" -> (Flee, neuron.Value)
    | _ -> raise (ArgumentException())

let getOutputNeuronNames () =
    [ "a"; "d"; "f" ]