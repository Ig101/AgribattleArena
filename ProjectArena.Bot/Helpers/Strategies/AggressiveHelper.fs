module ProjectArena.Bot.Helpers.Strategies.AggressiveHelper
open ProjectArena.Bot.Models.Dtos
open ProjectArena.Bot.Models.Neural
open ProjectArena.Bot.Models.States
open ProjectArena.Bot.Helpers.Strategies.StrategyHelper

let private getActionOnTiles (scene: Scene) (actor: ActorDto) () =
    let findAttackPositionWithNoDecoration (map: ActionPosition list) () =
        map
        |> List.filter (fun p -> not p.IsDecorationOnWay && p.EnemyAllowedActionWithPriority.IsSome)
        |> Option.create
        |> Option.filter (fun m -> not m.IsEmpty)
        |> Option.map (List.minBy (fun p -> p.Steps))
        |> Option.map (convertMapPositionToAction scene actor false)
    let findAttackPosition (map: ActionPosition list) () =
        map
        |> List.filter (fun p -> p.IsDecorationOnWay && p.EnemyAllowedActionWithPriority.IsSome)
        |> Option.create
        |> Option.filter (fun m -> not m.IsEmpty)
        |> Option.map (List.minBy (fun p -> p.Steps))
        |> Option.map (convertMapPositionToAction scene actor false)
    let findAttackPositionCloseToEnemy (map: ActionPosition list) () =
        map
        |> List.filter (fun p -> p.EnemyAllowedActionWithPriority.IsNone)
        |> Option.create
        |> Option.filter (fun m -> not m.IsEmpty)
        |> Option.map (List.minBy (fun p -> p.RangeTillNearestEnemy + (match p.IsDecorationOnWay with | true -> 10.0 | _ -> 0.0)))
        |> Option.map (convertMapPositionToAction scene actor false)

    let map = calculatePaths scene actor
    ()
    |> findAttackPositionWithNoDecoration map
    |> Option.orElseWith (findAttackPosition map)
    |> Option.orElseWith (findAttackPositionCloseToEnemy map)

let getAggressiveAction (scene: Scene, actor: ActorDto) =
    tryGetSummonNearlyAvailable scene actor
    |> Option.orElseWith (getActionOnTiles scene actor)
    |> Option.defaultValue SceneAction.Wait