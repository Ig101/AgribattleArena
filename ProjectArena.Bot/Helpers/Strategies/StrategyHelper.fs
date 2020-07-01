module ProjectArena.Bot.Helpers.Strategies.StrategyHelper
open System
open ProjectArena.Bot.Models.Dtos
open ProjectArena.Bot.Models.States
open ProjectArena.Bot.Models.Neural

let maxStepsMultiplier = 2.0

let moveSkills =
    [
        "charge", 0.1
        "mistwalk", 0.9
    ]

let meleeSkills =
    [
        "magicMissle", 0.5
        "warden", 0.5
    ]

let rangeSkills =
    [
        "magicMissle", 0.5
        "warden", 0.5
        "charge", 0.9
    ]

let buffSkills =
    [
        "empower", 0.9
    ]

let summonSkills =
    [
        "mistpact", 0.9
    ]
 
let private tryGetTile (scene: Scene) (x: int, y: int) =
    match x >= scene.TilesetWidth || x < 0 || y >= scene.TilesetHeight || y < 0 with
    | true -> None
    | false -> Some (scene.Tiles |> Seq.find (fun t -> t.X = x && t.Y = y))

let private bindTileToTileWithIsDecoration (previousSquare: ActionPosition) (scene: Scene) (tile: TileDto) =
    let decorationOpt = tile.TempActorId |> Option.bind (fun id -> scene.Decorations |> Seq.tryFind (fun d -> d.Id = id))
    match (decorationOpt, tile.TempActorId) with
    | (None, Some id) -> None
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

let private calculateAllowedActionsAndRanges (x: int, y: int) =
    // TODO
    ([], 0.0, 0.0)

let rec private calculateNextSquare
    (allies: ActorDto list, enemies: ActorDto list)
    (actor: ActorDto, scene: Scene)
    (initialTile: TileDto)
    (x: int, y: int)
    (previousSquare: ActionPosition)
    (allSquares: ActionPosition list) =

    let nextSteps = previousSquare.Steps + 1
    tryGetTile scene (x, y)
    |> Option.filter (fun tile ->
        actor.CanMove &&
        not tile.Unbearable &&
        Math.Abs(tile.Height - initialTile.Height) >= 10.0f &&
        float nextSteps > maxStepsMultiplier * float scene.TurnTime)
    |> Option.bind (bindTileToTileWithIsDecoration previousSquare scene)
    |> Option.bind (bindExistingTile nextSteps allSquares)
    |> Option.map (fun (tile, isDecoration, allSquares) ->
        let allowedActions, rangeTillAlly, rangeTillEnemy = calculateAllowedActionsAndRanges (x, y)
        let tempSquare = {
            X = x
            Y = y
            Steps = nextSteps
            IsDecorationOnWay = isDecoration
            ParentSquares = previousSquare.ParentSquares |> List.append [previousSquare]
            AllowedActionsWithPriority = allowedActions
            RangeTillNearestAlly = rangeTillAlly
            RangeTillNearestEnemy = rangeTillEnemy
        }
        let mutable allSquaresMutable = allSquares |> List.append [tempSquare]

        [ (-1, 0) ; (1, 0) ; (0, -1) ; (0, 1)]
        |> List.iter (fun (newX, newY) ->
            allSquaresMutable <- calculateNextSquare (allies, enemies) (actor, scene) tile (newX, newY) tempSquare allSquaresMutable)
        
        allSquaresMutable
        )
    |> Option.defaultValue allSquares

let calculatePaths (scene: Scene) (actor: ActorDto): ActionPosition list =
    let allowedActions, rangeTillAlly, rangeTillEnemy = calculateAllowedActionsAndRanges (actor.X, actor.Y)
    let actorSquare = {
        X = actor.X
        Y = actor.Y
        Steps = 0
        IsDecorationOnWay = false
        ParentSquares = []
        AllowedActionsWithPriority = allowedActions
        RangeTillNearestAlly = rangeTillAlly
        RangeTillNearestEnemy = rangeTillEnemy
    }
    let allies =
        scene.Actors
        |> Seq.filter (fun a -> actor.Owner.IsSome && a.Owner.IsSome && (actor.Owner = a.Owner || (actor.Owner.Value.Team.IsSome && a.Owner.Value.Team = actor.Owner.Value.Team)))
        |> Seq.toList
    let enemies =
        scene.Actors
        |> Seq.filter (fun a -> actor.Owner.IsNone || a.Owner.IsNone || (actor.Owner <> a.Owner && (actor.Owner.Value.Team.IsNone || a.Owner.Value.Team <> actor.Owner.Value.Team)))
        |> Seq.toList
    let tile = scene.Tiles |> Seq.find (fun t -> t.X = actor.X && t.Y = actor.Y)
    let mutable allSquaresMutable = [actorSquare]

    [ (-1, 0) ; (1, 0) ; (0, -1) ; (0, 1)]
    |> List.iter (fun (newX, newY) ->
        allSquaresMutable <- calculateNextSquare (allies, enemies) (actor, scene) tile (newX, newY) actorSquare allSquaresMutable)

    allSquaresMutable