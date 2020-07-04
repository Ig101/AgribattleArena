module ProjectArena.Bot.Helpers.Strategies.DefenciveHelper
open ProjectArena.Bot.Models.Dtos
open ProjectArena.Bot.Models.Neural
open ProjectArena.Bot.Models.States
open ProjectArena.Bot.Helpers.Strategies.StrategyHelper

let private getAllynessOfAction (position: ActionPosition) =
    let allyPriority = position.AllyAllowedActionWithPriority |> Option.map (fun (_, p) -> p) |> Option.defaultValue 0.0
    let enemyPriority = position.EnemyAllowedActionWithPriority |> Option.map (fun (_, p) -> p) |> Option.defaultValue 0.0
    allyPriority >= enemyPriority

let private getActionOnTiles (scene: Scene) (actor: ActorDto) () =
    let findActionPositionWithNoDecoration (map: ActionPosition list) () =
        map
        |> List.filter (fun p -> not p.IsDecorationOnWay && (p.EnemyAllowedActionWithPriority.IsSome || p.AllyAllowedActionWithPriority.IsSome))
        |> Option.create
        |> Option.filter (fun m -> not m.IsEmpty)
        |> Option.map (List.minBy (fun p -> p.Steps))
        |> Option.map (fun p -> p |> convertMapPositionToAction scene actor (getAllynessOfAction p))
    let findActionPosition (map: ActionPosition list) () =
        map
        |> List.filter (fun p -> p.IsDecorationOnWay && (p.EnemyAllowedActionWithPriority.IsSome || p.AllyAllowedActionWithPriority.IsSome))
        |> Option.create
        |> Option.filter (fun m -> not m.IsEmpty)
        |> Option.map (List.minBy (fun p -> p.Steps))
        |> Option.map (fun p -> p |> convertMapPositionToAction scene actor (getAllynessOfAction p))
    let findActionPositionCloseToAlly (map: ActionPosition list) () =
        map
        |> List.filter (fun p -> p.EnemyAllowedActionWithPriority.IsNone && p.AllyAllowedActionWithPriority.IsNone)
        |> List.sortBy (fun p -> p.Steps)
        |> Option.create
        |> Option.filter (fun m -> not m.IsEmpty)
        |> Option.map (List.minBy (fun p -> p.RangeTillNearestAlly + (match p.IsDecorationOnWay with | true -> 10.0 | _ -> 0.0)))
        |> Option.map (fun p -> p |> convertMapPositionToAction scene actor false)

    let map = calculatePaths scene actor
    ()
    |> findActionPositionWithNoDecoration map
    |> Option.orElseWith (findActionPosition map)
    |> Option.orElseWith (findActionPositionCloseToAlly map)

let getDefenciveAction (scene: Scene, actor: ActorDto) =
    tryGetSummonNearlyAvailable scene actor
    |> Option.orElseWith (getActionOnTiles scene actor)
    |> Option.defaultValue SceneAction.Wait