module ProjectArena.Bot.Helpers.Neural.OutputHelper
open System
open ProjectArena.Bot.Models.Neural

let private skillsWithShortNames =
    [
        "b", "barrier"
        "bs", "bloodsphere"
        "c", "charge"
        "e", "empower"
        "mm", "magicMissle"
        "mp", "mistpact"
        "ms", "mistShot"
        "mw", "mistwalk"
        "o", "offspring"
        "pp", "powerplace"
        "sa", "sacrifice"
        "sh", "shot"
        "sl", "slash"
        "w", "wand"
        "wa", "warden"
    ] |> Map.ofList

let convertCommandNeuronToAction (neuron: Neuron) =
    let typeSymbol = neuron.Name.[0]
    match typeSymbol with
    | 'o' ->
        let x = Int32.Parse (neuron.Name.Substring (1, 1))
        let y = Int32.Parse (neuron.Name.Substring (2, 1))
        let command = neuron.Name.Substring 3
        match command with
        | "m" -> Move (x, y)
        | _ -> Cast (skillsWithShortNames.[command], x, y)
    | _ -> raise (ArgumentException())

let convertMagnifyingNeuronToAction (neuron: Neuron) =
    let typeSymbol = neuron.Name.[0]
    match typeSymbol with
    | 'm' ->
        let x = Int32.Parse (neuron.Name.Substring (1, 1))
        let y = Int32.Parse (neuron.Name.Substring (2, 1))
        Proceed (x, y)
    | 'w' -> Wait
    | _ -> raise (ArgumentException())

    
let private getCommandOutputNeuronNamesFromPosition (x: int, y: int) =
    skillsWithShortNames
    |> Map.toList
    |> List.map (fun (key, _) -> key)
    |> List.append [ "m" ]
    |> List.map (fun key -> sprintf "o%i%i%s" x y key)

let getCommandOutputNeuronNames () =
    [0..15]
    |> List.map (fun p -> (p % 4, p / 4))
    |> List.collect (fun c -> c |> getCommandOutputNeuronNamesFromPosition)

let getMagnifyingOutputNeuronNames () =
    [0..27]
    |> List.map (fun p -> sprintf "m%i%i" (p % 7) (p / 7))
    |> List.append ([ "w" ])