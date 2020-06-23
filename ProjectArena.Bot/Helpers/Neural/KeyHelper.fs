module ProjectArena.Bot.Helpers.Neural.KeyHelper
open ProjectArena.Bot.Models.Neural
open ProjectArena.Bot.Models.States

let private getKeyValue (shift: int * int) (index: int, t: KeyNeuronType) (scene: Scene) =
    // TODO implement calculation
    1.0

let private getKeyNeuron (shift: int * int) (scene: Scene option) (index: int) =
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
            Value = scene |> Option.map (getKeyValue shift (index, t)) |> Option.defaultValue 0.0
        })

let getKeyInputNeurons (shift: int * int) (scene: Scene option) =
    [0..11]
    |> List.collect (fun k -> k |> getKeyNeuron shift scene)

let getMagnifyingKeyInputNeurons (scene: Scene option) =
    getKeyInputNeurons (0, 0) scene