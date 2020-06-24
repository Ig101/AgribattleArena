module ProjectArena.Bot.Helpers.Neural.SelfHelper
open ProjectArena.Bot.Models.Neural
open ProjectArena.Bot.Models.States
open ProjectArena.Bot.Helpers.SceneHelper

let private getSelfValue (shiftX: int, shiftY: int) (t: SelfNeuronType) (scene: Scene) =
    scene |> tryGetCurrentActorAndHisOwner
    |> Option.map (fun (actor, owner) ->
        match t with
        | XShift -> getXShift shiftX (Some actor)
        | YShift -> getYShift shiftY (Some actor)
        | Vulnerable -> getVulnerable (Some actor)
        | Damaged -> getDamaged (Some actor)
        | Mobile -> getMobile (Some actor)
        | Tough -> getTough (Some actor)
        | ActionPoints -> float actor.ActionPoints / 8.0
    ) |> Option.defaultValue 0.0

let getSelfNeuron (shift: int * int) (scene: Scene option) =
    [ XShift; YShift; Vulnerable; Damaged; Mobile; Tough; ActionPoints ]
    |> List.map (fun t ->
        let modifier = match t with
                       | XShift -> "x"
                       | YShift -> "y"
                       | Vulnerable -> "v"
                       | Damaged -> "d"
                       | Mobile -> "m"
                       | Tough -> "t"
                       | ActionPoints -> "a"
        {
            Name = sprintf "s%s" modifier
            Value = scene |> Option.map (getSelfValue shift t) |> Option.defaultValue 0.0
        })


let getMagnifyingSelfNeuron (scene: Scene option) =
    getSelfNeuron (0, 0) scene