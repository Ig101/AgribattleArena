module ProjectArena.Bot.Helpers.Strategies.FleeHelper
open ProjectArena.Bot.Models.Dtos
open ProjectArena.Bot.Models.Neural
open ProjectArena.Bot.Models.States
open ProjectArena.Bot.Helpers.Strategies.StrategyHelper

let getFleeAction (scene: Scene, actor: ActorDto) =
    let findBuffPositionWithNoDecoration (map: ActionPosition list) () =
        map
        |> List.filter (fun p -> not p.IsDecorationOnWay && p.AllyAllowedActionWithPriority.IsSome)
        |> Option.create
        |> Option.filter (fun m -> not m.IsEmpty)
        |> Option.map (List.minBy (fun p -> p.Steps))
        |> Option.map (fun p -> p |> convertMapPositionToAction scene actor true)
    let findBuffPosition (map: ActionPosition list) () =
        map
        |> List.filter (fun p -> p.IsDecorationOnWay && p.AllyAllowedActionWithPriority.IsSome)
        |> Option.create
        |> Option.filter (fun m -> not m.IsEmpty)
        |> Option.map (List.minBy (fun p -> p.Steps))
        |> Option.map (fun p -> p |> convertMapPositionToAction scene actor true)
    let findActionPositionFarFromEnemy (map: ActionPosition list) () =
        map
        |> List.filter (fun p -> p.AllyAllowedActionWithPriority.IsNone)
        |> List.sortBy (fun p -> p.Steps)
        |> Option.create
        |> Option.filter (fun m -> not m.IsEmpty)
        |> Option.map (List.maxBy (fun p -> p.RangeTillNearestEnemy + (match p.IsDecorationOnWay with | true -> -10.0 | _ -> 0.0)))
        |> Option.map (fun p -> p |> convertMapPositionToAction scene actor true)

    let map = calculatePaths scene actor
    ()
    |> findBuffPositionWithNoDecoration map
    |> Option.orElseWith (findBuffPosition map)
    |> Option.orElseWith (findActionPositionFarFromEnemy map)
    |> Option.defaultValue SceneAction.Wait