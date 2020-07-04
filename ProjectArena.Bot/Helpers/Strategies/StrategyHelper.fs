module ProjectArena.Bot.Helpers.Strategies.StrategyHelper
open System
open ProjectArena.Bot.Models.Dtos
open ProjectArena.Bot.Models.States
open ProjectArena.Bot.Models.Neural
open System.Diagnostics
open ProjectArena.Bot.Helpers.ActionsHelper

let maxStepsMultiplier = 2.0
let maxSteps = 14

let meleeSkills =
    [
        "magicMissle", 0.6, None
        "warden", 0.6, None
        "shot", 0.1, None
        "slash", 0.4, None
        "mistShot", 0.1, None
        "mistSlash", 0.4, None
        "wand", 0.1, None
    ]

let rangeSkills =
    [
        "magicMissle", 0.6, None
        "warden", 0.6, None
        "charge", 0.9, Some "stun"
        "shot", 0.4, None
        "mistShot", 0.4, None
        "wand", 0.4, None
    ]

let buffSkills =
    [
        "empower", 0.91, Some "empower"
    ]

let summonSkillsPrioritized =
    [
        "mistpact"
    ]

let getRightSkillsArrayByPositionAndTarget (x: int, y: int) (target: ActorDto) =
    match Math.Abs (x - target.X) + Math.Abs (y - target.Y) with
    | range when range > 4 -> rangeSkills
    | _ -> meleeSkills
 
let private tryGetTile (scene: Scene) (x: int, y: int) =
    match x >= scene.TilesetWidth || x < 0 || y >= scene.TilesetHeight || y < 0 with
    | true -> None
    | false -> Some (scene.Tiles |> Seq.find (fun t -> t.X = x && t.Y = y))

let private bindTileToTileWithIsDecoration (previousSquare: ActionPosition) (scene: Scene) (tile: TileDto) =
    let decorationOpt = tile.TempActorId |> Option.bind (fun id -> scene.Decorations |> Seq.tryFind (fun d -> d.Id = id))
    match (decorationOpt, tile.TempActorId) with
    | (None, Some _) -> None
    | _ ->
        Some (tile, decorationOpt.IsSome || previousSquare.IsDecorationOnWay)
   
let private bindExistingTile (nextSteps: int) (allSquares: ActionPosition list) (tile: TileDto, isDecoration: bool) =
    let existingSquareOpt = allSquares |> Seq.tryFind(fun s -> s.X = tile.X && s.Y = tile.Y);
    match existingSquareOpt with
    | Some existingSquare when (nextSteps >= existingSquare.Steps || not existingSquare.IsDecorationOnWay && isDecoration) ->
        None
    | Some existingSquare ->
        Some (tile, isDecoration, allSquares |> List.filter(fun s -> existingSquare <> s))
    | None ->
        Some (tile, isDecoration, allSquares)

let private tryGetActionAvailable (scene: Scene) (actor: ActorDto) (x: int, y: int) (skill: SkillDto) (ally: bool) (target: ActorDto) =
    match ally with
    | true -> buffSkills
    | false -> getRightSkillsArrayByPositionAndTarget (x, y) target
    |> Seq.tryFind (fun (v, _, _) -> v = skill.NativeId)
    |> Option.map (fun (_, c, b) ->
        Cast (skill.NativeId, target.X, target.Y),
            c +
            (match actor = target with | true -> -0.1 | false -> 0.0) +
            (match b.IsSome && target.Buffs |> Seq.exists (fun aB -> aB.NativeId = b.Value) with | true -> -1.0 | false -> 0.0))
    |> Option.filter (fun (a, _) -> isActionAllowedByPosition (x, y) scene (actor, actor.Owner.Value) a)

let private tryGetActionAvailableBulk (scene: Scene) (actors: ActorDto list, ally: bool) (actor: ActorDto) (x: int, y: int) (skill: SkillDto) =
    actors
    |> Seq.map (tryGetActionAvailable scene actor (x, y) skill ally)
    |> Seq.toList

let private calculateAllyAndEnemyAllowedActionsAndMinRanges (scene: Scene) (allies: ActorDto list, enemies: ActorDto list) (actor: ActorDto) (x: int, y: int) =
    let getRankedActions (actorsAndAlly: ActorDto list * bool) =
        actor.Skills
        |> Seq.collect (tryGetActionAvailableBulk scene actorsAndAlly actor (x, y))
        |> Seq.append (tryGetActionAvailableBulk scene actorsAndAlly actor (x, y) actor.AttackingSkill.Value)
        |> Seq.choose id
        |> Seq.toList
        |> Option.create
        |> Option.filter (fun m -> not m.IsEmpty)
        |> Option.map (Seq.maxBy (fun (_, p) -> p))
    let allyMinRange = match allies |> List.filter (fun a -> a <> actor) with
                       | allies when allies |> List.isEmpty -> 0.0
                       | allies -> allies |> List.map (fun a -> rangeBetween (x, y, a.X, a.Y)) |> List.min
    let enemyMinRange = match enemies with
                        | enemies when enemies |> List.isEmpty -> 0.0
                        | enemies -> enemies |> List.map (fun a -> rangeBetween (x, y, a.X, a.Y)) |> List.min
    (getRankedActions(allies, true), allyMinRange, getRankedActions(enemies, false), enemyMinRange)

let rec private calculateNextSquare
    (alliesAndEnemies: ActorDto list * ActorDto list)
    (actor: ActorDto, scene: Scene)
    (initialTile: TileDto)
    (x: int, y: int)
    (previousSquare: ActionPosition)
    (allSquares: ActionPosition list) =

    let nextSteps = previousSquare.Steps + 1
    tryGetTile scene (x, y)
    |> Option.filter (fun tile ->
        nextSteps <= maxSteps &&
        float nextSteps <= maxStepsMultiplier * float scene.TurnTime &&
        actor.CanMove &&
        not tile.Unbearable &&
        Math.Abs(tile.Height - initialTile.Height) < 10.0f)
    |> Option.bind (bindTileToTileWithIsDecoration previousSquare scene)
    |> Option.bind (bindExistingTile nextSteps allSquares)
    |> Option.map (fun (tile, isDecoration, allSquares) ->
        let allowedActionAlly, rangeTillAlly, allowedActionEnemy, rangeTillEnemy =
            calculateAllyAndEnemyAllowedActionsAndMinRanges scene alliesAndEnemies actor (x, y)
        let tempSquare = {
            X = x
            Y = y
            Steps = nextSteps
            IsDecorationOnWay = isDecoration
            ParentSquares = [previousSquare] |> List.append previousSquare.ParentSquares 
            AllyAllowedActionWithPriority = allowedActionAlly
            EnemyAllowedActionWithPriority = allowedActionEnemy
            RangeTillNearestAlly = rangeTillAlly
            RangeTillNearestEnemy = rangeTillEnemy
        }
        let mutable allSquaresMutable = allSquares |> List.append [tempSquare]

        [ (-1, 0) ; (1, 0) ; (0, -1) ; (0, 1)]
        |> List.iter (fun (newX, newY) ->
            allSquaresMutable <- calculateNextSquare alliesAndEnemies (actor, scene) tile (x + newX, y + newY) tempSquare allSquaresMutable)
        
        allSquaresMutable
        )
    |> Option.defaultValue allSquares

let calculatePaths (scene: Scene) (actor: ActorDto): ActionPosition list =
    let allies =
        scene.Actors
        |> Seq.filter (fun a -> actor.Owner.IsSome && a.Owner.IsSome && (actor.Owner = a.Owner || (actor.Owner.Value.Team.IsSome && a.Owner.Value.Team = actor.Owner.Value.Team)))
        |> Seq.toList
    let enemies =
        scene.Actors
        |> Seq.filter (fun a -> actor.Owner.IsNone || a.Owner.IsNone || (actor.Owner <> a.Owner && (actor.Owner.Value.Team.IsNone || a.Owner.Value.Team <> actor.Owner.Value.Team)))
        |> Seq.toList
    let allowedActionAlly, rangeTillAlly, allowedActionEnemy, rangeTillEnemy =
        calculateAllyAndEnemyAllowedActionsAndMinRanges scene (allies, enemies) actor (actor.X, actor.Y)
    let actorSquare = {
        X = actor.X
        Y = actor.Y
        Steps = 0
        IsDecorationOnWay = false
        ParentSquares = []
        AllyAllowedActionWithPriority = allowedActionAlly
        EnemyAllowedActionWithPriority = allowedActionEnemy
        RangeTillNearestAlly = rangeTillAlly
        RangeTillNearestEnemy = rangeTillEnemy
    }
    let tile = scene.Tiles |> Seq.find (fun t -> t.X = actor.X && t.Y = actor.Y)
    let mutable allSquaresMutable = [actorSquare]

    [ (-1, 0) ; (1, 0) ; (0, -1) ; (0, 1) ]
    |> List.iter (fun (newX, newY) ->
        allSquaresMutable <- calculateNextSquare (allies, enemies) (actor, scene) tile (actor.X + newX, actor.Y + newY) actorSquare allSquaresMutable)

    allSquaresMutable

let tryGetSummonNearlyAvailable (scene: Scene) (actor: ActorDto) =
    let tryReturnAvailableAction (skill: SkillDto) (xShift: int, yShift: int) =
        let action = Cast (skill.NativeId, actor.X + xShift, actor.Y + yShift)
        match isActionAllowedByPosition (actor.X, actor.Y) scene (actor, actor.Owner.Value) action with
        | true -> Some action
        | false -> None
    let getSkillsAvailableToCastOnPositions (skill: SkillDto) =
        [ (-1, 0) ; (1, 0) ; (0, -1) ; (0, 1) ]
        |> List.map (tryReturnAvailableAction skill)
    summonSkillsPrioritized
    |> List.map (fun s ->
        actor.Skills |> Seq.tryFind (fun actS -> actS.NativeId = s))
    |> List.choose id
    |> List.collect getSkillsAvailableToCastOnPositions
    |> List.choose id
    |> List.tryHead

let convertMapPositionToAction (scene: Scene) (actor: ActorDto) (ally: bool) (position: ActionPosition) =
    let tryGetAvailableAction (targetX: int, targetY: int) (skill: SkillDto) =
        meleeSkills
        |> Seq.tryFind (fun (v, _, _) -> v = skill.NativeId)
        |> Option.map (fun (_, c, b) -> Cast (skill.NativeId, targetX, targetY), c + (match b with | Some b -> -1.0 | None -> 0.0))
        |> Option.filter (fun (a, _) -> isActionAllowed scene (actor, actor.Owner.Value) a)
    let calculateNextPositionAction (position: ActionPosition) =
        match position.IsDecorationOnWay with
        | true ->
            match actor.Skills
                  |> Seq.map (tryGetAvailableAction (position.X, position.Y))
                  |> Seq.append ([ tryGetAvailableAction (position.X, position.Y) actor.AttackingSkill.Value])
                  |> Seq.choose id
                  |> Seq.toList
                  |> Option.create
                  |> Option.filter (fun m -> not m.IsEmpty)
                  |> Option.map (Seq.maxBy (fun (_, p) -> p)) with
            | Some (s, _) -> s
            | None -> Wait
        | false -> Move (position.X, position.Y)
    match (position) with
    | position when actor.X = position.X && actor.Y = position.Y ->
        match (match ally with | true -> position.AllyAllowedActionWithPriority | false -> position.EnemyAllowedActionWithPriority) with
        | Some (action, _) -> action
        | None -> Wait
    | position when position.ParentSquares.Length = 1 ->
        position |> calculateNextPositionAction
    | _ ->
        position.ParentSquares.[1] |> calculateNextPositionAction