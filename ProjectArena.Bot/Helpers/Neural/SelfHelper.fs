module ProjectArena.Bot.Helpers.Neural.SelfHelper
open ProjectArena.Bot.Models.Neural
open ProjectArena.Bot.Models.States

let private getSelfValue (shift: int * int) (t: SelfNeuronType) (scene: Scene) =
    // TODO implement calculation
    1.0

let getSelfNeuron (shift: int * int) (scene: Scene option) =
    [ XShift; YShift; Vulnerable; Damaged; Mobile; Tough; ActionPoints; Ranger; Summoner; Buffer; Melee ]
    |> List.map (fun t ->
        let modifier = match t with
                       | XShift -> "x"
                       | YShift -> "y"
                       | Vulnerable -> "v"
                       | Damaged -> "d"
                       | Mobile -> "m"
                       | Tough -> "t"
                       | ActionPoints -> "a"
                       | Ranger -> "r"
                       | Summoner -> "s"
                       | Buffer -> "b"
                       | Melee -> "e"
        {
            Name = sprintf "s%s" modifier
            Value = scene |> Option.map (getSelfValue shift t) |> Option.defaultValue 0.0
        })


let getMagnifyingSelfNeuron (scene: Scene option) =
    getSelfNeuron (0, 0) scene