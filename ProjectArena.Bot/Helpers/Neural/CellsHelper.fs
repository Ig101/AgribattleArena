module ProjectArena.Bot.Helpers.Neural.CellsHelper
open ProjectArena.Bot.Models.Neural
open ProjectArena.Bot.Models.States

let private getMagnifyingCellValue (x: int, y: int, t: CellNeuronType) (scene: Scene, actorId: int) =
    // TODO implement calculation
    1.0

let private getCommandCellValue (shiftX: int, shiftY: int) (x: int, y: int, t: CellNeuronType) (scene: Scene, actorId: int) =
    // TODO implement calculation
    1.0

let private getCellValue (magnified: (int * int) option) info (sceneAndActorId: Scene * int) =
    match magnified with
    | None -> sceneAndActorId |> getMagnifyingCellValue info
    | Some v -> sceneAndActorId |> getCommandCellValue v info

let private getCellNeuron (magnified: (int * int) option) (sceneAndActorId: (Scene * int) option) (x: int, y: int) =
    [ Dangerous; Friendly; CellNeuronType.Known; Active ]
    |> List.map (fun t ->
        let modifier = match t with
                       | Dangerous -> "d"
                       | Friendly -> "f"
                       | CellNeuronType.Known -> "k"
                       | Active -> "a"
        {
            Name = sprintf "c%i%i%s" x y modifier
            Value = sceneAndActorId |> Option.map (getCellValue magnified (x, y, t)) |> Option.defaultValue 0.0
        })
            
let getMagnifyingCellsInputNeurons (sceneAndActorId: (Scene * int) option) =
    [0..27]
    |> List.map (fun p -> (p % 7, p / 7))
    |> List.collect (fun c -> c |> getCellNeuron None sceneAndActorId)

let getCommandCellsInputNeurons (shift: int * int) (sceneAndActorId: (Scene * int) option) =
    [0..15]
    |> List.map (fun p -> (p % 4, p / 4))
    |> List.collect (fun c -> c |> getCellNeuron (Some shift) sceneAndActorId)
