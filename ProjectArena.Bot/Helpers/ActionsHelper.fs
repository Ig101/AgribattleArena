module ProjectArena.Bot.Helpers.ActionsHelper
open System
open ProjectArena.Bot.Models.States
open ProjectArena.Bot.Models.Dtos
open ProjectArena.Bot.Models.Neural

let rangeBetween (x1: int, y1: int, x2: int, y2: int) =
    Math.Sqrt(float (((x2 - x1) * (x2 - x1)) + ((y2 - y1) * (y2 - y1))));

let angleBetween (x1: int, y1: int, x2: int, y2: int) =
    Math.Atan2(float (y2 - y1), float (x2 - x1))

let private isMoveAllowedByTile (scene: Scene) (actorX: int, actorY: int) (x: int, y: int) =
    let tile = scene.Tiles |> Seq.find (fun t -> t.X = x && t.Y = y)
    let tempTile = scene.Tiles |> Seq.find (fun t -> t.X = actorX && t.Y = actorY)
    tile.TempActorId = None && not tile.Unbearable && Math.Abs(tile.Height - tempTile.Height) < 10.0f

let private checkMilliness (scene: Scene) (actorX: int, actorY: int) (melee: bool) (target: TileDto) =
    let range = rangeBetween(actorX, actorY, target.X, target.Y);
    let mutable incrementingRange = 0.0;
    let mutable freeWay = true
    let angleBetween = angleBetween(actorX, actorY, target.X, target.Y);
    let sin = Math.Sin(angleBetween);
    let cos = Math.Cos(angleBetween);
    let mutable currentTile = scene.Tiles |> Seq.find (fun t -> t.X = actorX && t.Y = actorY)
    while freeWay && incrementingRange <= range do
        incrementingRange <- incrementingRange + 1.0;
        let nextTarget = match incrementingRange >= range with
                         | true -> target
                         | false ->
                            let nextXFloat = float actorX + (float incrementingRange * cos);
                            let nextYFloat = float actorY + (float incrementingRange * sin);
                            let nextX = int (Math.Round(nextXFloat, MidpointRounding.AwayFromZero));
                            let nextY = int (Math.Round(nextYFloat, MidpointRounding.AwayFromZero));
                            scene.Tiles |> Seq.find (fun t -> t.X = nextX && t.Y = nextY)
        
        if nextTarget <> currentTile then
            if nextTarget.Height - currentTile.Height >= 10.0f || (melee && currentTile.Height -  nextTarget.Height >= 10.0f) || (nextTarget <> target && nextTarget.TempActorId.IsSome) ||  nextTarget.Unbearable then
                freeWay <- false
            currentTile <- nextTarget;

    freeWay

let private isSkillAllowedByTile (scene: Scene) (actor: ActorDto, player: PlayerDto) (actorX: int, actorY: int) (skill: SkillDto, x: int, y: int) =
    let tile = scene.Tiles |> Seq.find (fun t -> t.X = x && t.Y = y)
    let targetActor = tile.TempActorId |> Option.bind (fun aId -> scene.Actors |> Seq.tryFind (fun a -> aId = a.Id))
    let targetActorOwner = targetActor |> Option.bind (fun a -> a.Owner)
    let targetDecoration = tile.TempActorId |> Option.bind (fun dId -> scene.Decorations |> Seq.tryFind (fun d -> dId = d.Id))
    let allies = skill.AvailableTargets.Allies && targetActor.IsSome && targetActor.Value.Id <> actor.Id && player.Team = (targetActorOwner |> Option.bind (fun p -> p.Team))
    let enemies = skill.AvailableTargets.NotAllies && targetActor.IsSome && targetActor.Value.Id <> actor.Id && (player.Team.IsNone || player.Team <> (targetActorOwner |> Option.bind (fun p -> p.Team)))
    let decorations = skill.AvailableTargets.Decorations && targetDecoration.IsSome
    let self = skill.AvailableTargets.Self && targetActor.IsSome && targetActor.Value.Id = actor.Id
    let bearable = skill.AvailableTargets.Bearable && tile.TempActorId.IsNone && not tile.Unbearable
    let unbearable = skill.AvailableTargets.Unbearable && tile.TempActorId.IsNone && tile.Unbearable
    (allies || enemies || decorations || self || bearable || unbearable) && (not skill.OnlyVisibleTargets || checkMilliness scene (actorX, actorY) (skill.Range <= 1) tile)

let isActionAllowedByPosition (actorX, actorY) (scene: Scene) (actor: ActorDto, player: PlayerDto) (action: SceneAction) =
    match action with
    | Wait ->
        true
    | Move (x, y) ->
        actor.CanMove && ((x = actorX && Math.Abs(y - actorY) = 1) || (y = actorY && Math.Abs(actorX - x) = 1)) && isMoveAllowedByTile scene (actorX, actorY) (x, y)
    | Cast (name, x, y) ->
        let skillOpt =
            actor.AttackingSkill
            |> Option.filter (fun s -> s.NativeId = name)
            |> Option.orElse (actor.Skills |> Seq.tryFind (fun s -> s.NativeId = name))
        match skillOpt with
        | None -> false
        | Some skill ->
            actor.ActionPoints >= skill.Cost && skill.PreparationTime <= 0.0f && actor.CanAct && rangeBetween(actorX, actorY, x, y) <= float skill.Range && isSkillAllowedByTile scene (actor, player) (actorX, actorY) (skill, x, y)


let isActionAllowed (scene: Scene) (actor: ActorDto, player: PlayerDto) (action: SceneAction) =
    isActionAllowedByPosition (actor.X, actor.Y) scene (actor, player) action

let private getAnyAvailableSkill (scene: Scene) (actor: ActorDto, player: PlayerDto) (x, y) =
    actor.AttackingSkill
        |> Option.filter (fun s -> isActionAllowed scene (actor, player) (Cast (s.NativeId, x, y)))
        |> Option.orElse (actor.Skills |> Seq.tryFind (fun s -> isActionAllowed scene (actor, player) (Cast (s.NativeId, x, y))))

let isAnyActionAllowedOnPosition (scene: Scene) (actorPlayer: ActorDto * PlayerDto) (x: int, y: int) =
    isActionAllowed scene actorPlayer (Move (x, y)) || (getAnyAvailableSkill scene actorPlayer (x, y)).IsSome

let isAnyActionAllowedOnBlock (scene: Scene) (actorPlayer: ActorDto * PlayerDto) (x: int, y: int) =
    ([0..15] |> List.tryFind (fun p -> isAnyActionAllowedOnPosition scene actorPlayer (x + p % 4, y + p / 4))) <> None