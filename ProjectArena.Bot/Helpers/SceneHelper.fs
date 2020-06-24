module ProjectArena.Bot.Helpers.SceneHelper
open System
open ProjectArena.Bot.Models.Dtos
open ProjectArena.Bot.Models.States

let strengthDifferenceCoefficient = 0.8

let normalize (value: float) =
    1.0 / (1.0 + Math.Pow (Math.E, -value))

let tryGetOwner (scene: Scene) (ownerId: string option) =
    ownerId |> Option.map (fun owner -> scene.Players |> Seq.find (fun p -> p.Id = owner))

let tryGetCurrentActorAndHisOwner (scene: Scene) =
    scene.TempActor 
    |> Option.bind (fun actor -> actor.OwnerId |> Option.map(fun ownerId -> (actor, scene.Players |> Seq.find (fun p -> p.Id = ownerId))))

let isAlly (currentPlayer: PlayerDto) (anotherPlayer: PlayerDto option) =
    match anotherPlayer with
    | None -> false
    | Some player ->
        currentPlayer.Team.IsSome && player.Team.IsSome && currentPlayer.Team.Value = player.Team.Value

let getActorPower (actor: ActorDto) =
    let maxHealth = actor.MaxHealth |> Option.map float |> Option.defaultValue 100.0
    let healthCoefficient = (actor.Health |> Option.map float |> Option.defaultValue 100.0) / float maxHealth
    let initiativeCoefficient = Math.Max (0.0, 1.0 - float (actor.InitiativePosition / actor.Initiative))
    let cdsCoefficient = match actor.Skills |> Seq.length with
                         | 0 -> 1.0
                         | _ -> Math.Max (0.0, 1.0 - (float (actor.Skills |> Seq.minBy (fun s -> s.PreparationTime)).PreparationTime) / 3.0)
    match actor.ExternalId with
    | Some _ ->
        healthCoefficient + initiativeCoefficient + cdsCoefficient
    | None ->
        let attackCoefficient = match actor.AttackingSkill with
                                | Some skill -> Math.Min (1.0, (float skill.Range) / 5.0 + 0.6)
                                | None -> 1.0
        attackCoefficient * maxHealth / 100.0 * (healthCoefficient + initiativeCoefficient + cdsCoefficient)



let getDecorationDanger (decoration: ActiveDecorationDto) =
    match decoration.NativeId with
            | "barrier" -> 0.0
            | _ -> 0.0

let getEffectDanger (effect: SpecEffectDto) =
    0.0

let getTileDanger (tile: TileDto) =
    match tile.NativeId with
        | "powerplace" -> 0.0
        | _ -> 0.0

let getDecorationHelp (decoration: ActiveDecorationDto) =
    match decoration.NativeId with
        | "barrier" -> 0.0
        | _ -> 0.0

let getEffectHelp (effect: SpecEffectDto) =
    0.0

let getTileHelp (tile: TileDto) =
    match tile.NativeId with
    | "powerplace" -> 1.0
    | _ -> 0.0


let getDamaged (actor: ActorDto option) =
    actor |> Option.map (fun a -> (a.Health |> Option.map float |> Option.defaultValue 100.0) / (a.MaxHealth |> Option.map float |> Option.defaultValue 100.0)) |> Option.defaultValue 0.0

let getVulnerable (actor: ActorDto option) =
    actor
    |> Option.map (fun a -> match a.CanAct with | true -> 0.0 | false -> 0.6 + match a.CanMove with | true -> 0.0 | false -> 0.4)
    |> Option.defaultValue 0.0
  

let getMobile (actor: ActorDto option) =
    actor
    |> Option.map (fun a ->
        a.Skills
        |> Seq.append (a.AttackingSkill |> Option.map (fun s -> [s]) |> Option.defaultValue [])
        |> Seq.map (fun s -> float s.Range / float s.Cost)
        |> Seq.max
        |> normalize
        )
    |> Option.defaultValue 0.0

let getTough (actor: ActorDto option) =
    actor
    |> Option.map (fun a ->
        let health = a.MaxHealth |> Option.map float |> Option.defaultValue 100.0
        normalize (health / 100.0))
    |> Option.defaultValue 0.0

let getKnown (actor: ActorDto option) =
    actor
    |> Option.map (fun a ->
        let healthCoefficient = match a.Health.IsSome with | true -> 0.3 | false -> 0.0
        let attackCoefficient = match a.AttackingSkill.IsSome with | true -> 0.4 | false -> 0.0
        let skillsCoefficient = float (a.Skills |> Seq.length) * 0.06
        healthCoefficient + attackCoefficient + skillsCoefficient)
    |> Option.defaultValue 1.0

let getXShift (shiftX: int) (actor: ActorDto option) =
    actor |> Option.map (fun a -> float (a.X - shiftX) / 28.0) |> Option.defaultValue 0.0

let getYShift (shiftY: int) (actor: ActorDto option) =
    actor |> Option.map (fun a -> float (a.Y - shiftY) / 16.0) |> Option.defaultValue 0.0