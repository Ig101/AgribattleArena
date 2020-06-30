module ProjectArena.Bot.Helpers.Neural.SelfHelper
open ProjectArena.Bot.Models.Neural
open ProjectArena.Bot.Models.States
open ProjectArena.Bot.Helpers.SceneHelper
open ProjectArena.Bot.Models.Dtos

let private skillsWithShortNames =
    [
        "b", "barrier"
        "bs", "bloodsphere"
        "c", "charge"
        "e", "empower"
        "mm", "magicMissle"
        "mp", "mistpact"
        "mw", "mistwalk"
        "o", "offspring"
        "pp", "powerplace"
        "sa", "sacrifice"
        "wa", "warden"
    ]

let private getRangedValue (attackSkillId: string) =
    match attackSkillId with
    | "shot" -> 1.0
    | "mistShot" -> 1.0
    | "wand" -> 1.0
    | "slash" -> 0.0
    | "mistSlash" -> 0.0
    | _ -> 0.0

let private getSelfValue (t: SelfNeuronType) (scene: Scene) =
    scene |> tryGetCurrentActorAndHisOwner
    |> Option.map (fun (actor, owner) ->
        match t with
        | XShift -> getXShift (Some actor)
        | YShift -> getYShift (Some actor)
        | Vulnerable -> getVulnerable (Some actor)
        | Damaged -> getDamaged (Some actor)
        | Mobile -> getMobile (Some actor)
        | Tough -> getTough (Some actor)
        | ActionPoints -> float actor.ActionPoints / 8.0
        | Ranged -> getRangedValue actor.AttackingSkill.Value.NativeId
        | Time -> float scene.TurnTime / 10.0
    ) |> Option.defaultValue 0.0

let getSelfNeuron (scene: Scene option) =
    [ XShift; YShift; Vulnerable; Damaged; Mobile; Tough; ActionPoints; Ranged; Time ]
    |> List.map (fun t ->
        let modifier = match t with
                       | XShift -> "x"
                       | YShift -> "y"
                       | Vulnerable -> "v"
                       | Damaged -> "d"
                       | Mobile -> "m"
                       | Tough -> "t"
                       | ActionPoints -> "a"
                       | Ranged -> "r"
                       | Time -> "s"
        {
            Name = modifier
            Value = scene |> Option.map (getSelfValue t) |> Option.defaultValue 0.0
        })

let private getSelfSkillNeuronValue (skillNativeId: string) (actor: ActorDto, scene: Scene) =
    let skillPreparationOpt = actor.Skills |> Seq.tryFind(fun s -> s.NativeId = skillNativeId) |> Option.map (fun s -> s.PreparationTime)
    match skillPreparationOpt with
    | Some time when time > 0.0f -> 0.5
    | Some time -> 1.0
    | None -> 0.0

let private bindSceneToActorAndScene (scene: Scene) =
    scene |> tryGetCurrentActorAndHisOwner |> Option.map (fun (a, p) -> (a, scene))

let getSelfSkillsNeurons (scene: Scene option) =
    skillsWithShortNames
    |> List.map (fun (id, name) ->
        {
            Name = id
            Value = scene |> Option.bind (bindSceneToActorAndScene) |> Option.map (getSelfSkillNeuronValue name) |> Option.defaultValue 0.0
        })