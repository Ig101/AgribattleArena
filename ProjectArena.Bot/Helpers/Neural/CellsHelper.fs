module ProjectArena.Bot.Helpers.Neural.CellsHelper
open ProjectArena.Bot.Models.Neural
open ProjectArena.Bot.Models.States
open ProjectArena.Bot.Models.Dtos

let private getMagnifyingCellValue (x: int, y: int, t: CellNeuronType) (scene: Scene) =
    // TODO implement calculation
    1.0

let private getCommandCellValue (shiftX: int, shiftY: int) (x: int, y: int, t: CellNeuronType) (scene: Scene) =
    // TODO implement calculation
    1.0

let private getCellValue (magnified: (int * int) option) info (scene: Scene) =
    match magnified with
    | None -> scene |> getMagnifyingCellValue info
    | Some v -> scene |> getCommandCellValue v info

let private getCellNeuron (magnified: (int * int) option) (scene: Scene option) (x: int, y: int) =
    [ Dangerous; Friendly; CellNeuronType.Known; Active ]
    |> List.map (fun t ->
        let modifier = match t with
                       | Dangerous -> "d"
                       | Friendly -> "f"
                       | CellNeuronType.Known -> "k"
                       | Active -> "a"
        {
            Name = sprintf "c%i%i%s" x y modifier
            Value = scene |> Option.map (getCellValue magnified (x, y, t)) |> Option.defaultValue 0.0
        })
            
let getMagnifyingCellsInputNeurons (scene: Scene option) =
    [0..27]
    |> List.map (fun p -> (p % 7, p / 7))
    |> List.collect (fun c -> c |> getCellNeuron None scene)

let getCommandCellsInputNeurons (shift: int * int) (scene: Scene option) =
    [0..15]
    |> List.map (fun p -> (p % 4, p / 4))
    |> List.collect (fun c -> c |> getCellNeuron (Some shift) scene)
