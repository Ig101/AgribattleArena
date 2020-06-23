module ProjectArena.Bot.Helpers.Neural.KeyHelper
open ProjectArena.Bot.Models.Neural
open ProjectArena.Bot.Models.States

let private getKeyValue (shift: int * int) (index: int, t: KeyNeuronType) (sceneAndActorId: Scene * int) =
    // TODO implement calculation
    1.0

let private getKeyNeuron (shift: int * int) (sceneAndActorId: (Scene * int) option) (index: int) =
    [ KeyNeuronType.XShift; KeyNeuronType.YShift; Harmful; Helpful; KeyNeuronType.Vulnerable; KeyNeuronType.Damaged; Known; Alive; KeyNeuronType.Mobile; KeyNeuronType.Tough ]
    |> List.map (fun t ->
        let modifier = match t with
                       | KeyNeuronType.XShift -> "x"
                       | KeyNeuronType.YShift -> "y"
                       | Harmful -> "e"
                       | Helpful -> "h"
                       | KeyNeuronType.Vulnerable -> "v"
                       | KeyNeuronType.Damaged -> "d"
                       | Known -> "k"
                       | Alive -> "a"
                       | KeyNeuronType.Mobile -> "m"
                       | KeyNeuronType.Tough -> "t"
        {
            Name = sprintf "k%0i%s" index modifier
            Value = sceneAndActorId |> Option.map (getKeyValue shift (index, t)) |> Option.defaultValue 0.0
        })

let getKeyInputNeurons (shift: int * int) (sceneAndActorId: (Scene * int) option) =
    [0..11]
    |> List.collect (fun k -> k |> getKeyNeuron shift sceneAndActorId)

let getMagnifyingKeyInputNeurons (sceneAndActorId: (Scene * int) option) =
    getKeyInputNeurons (0, 0) sceneAndActorId