module ProjectArena.Bot.Helpers.Neural.CellsHelper
open System
open ProjectArena.Bot.Models.Neural
open ProjectArena.Bot.Models.States
open ProjectArena.Bot.Models.Dtos
open ProjectArena.Bot.Helpers.SceneHelper
open System.Diagnostics

let actorShare = 0.6
let massActorShare = 0.8

let private calculateCellUnbearableParam (tile: TileDto) =
    if tile.Unbearable || tile.TempActorId.IsSome then
        1.0
    else
        0.0

let private calculateSectorDangerousParam
    (scene: Scene)
    (tiles: TileDto list, tileActors: ActorDto list, tileDecorations: ActiveDecorationDto list, tileEffects: SpecEffectDto list)
    (actor: ActorDto, player: PlayerDto) =

    let actorCoefficient =
        tileActors
        |> List.sumBy (fun enemyActor ->
            let ownerOpt = enemyActor.OwnerId |> tryGetOwner scene
            match isAlly player ownerOpt with
            | true -> 0.0
            | false ->
                Math.Min (1.0, getActorPower enemyActor / getActorPower actor * strengthDifferenceCoefficient)
        )
    let decorationCoefficient = tileDecorations |> List.sumBy getDecorationDanger
    let effectsCoefficient = tileEffects |> List.sumBy getEffectDanger
    let tilesCoefficient = tiles |> List.sumBy getTileDanger
    normalize ((actorCoefficient + decorationCoefficient) * massActorShare + (tilesCoefficient + effectsCoefficient) * (1.0 - massActorShare))

let private calculateSectorFriendlyParam
    (scene: Scene)
    (tiles: TileDto list, tileActors: ActorDto list, tileDecorations: ActiveDecorationDto list, tileEffects: SpecEffectDto list)
    (actor: ActorDto, player: PlayerDto) =

    let actorCoefficient =
        tileActors
        |> List.sumBy (fun enemyActor ->
            let ownerOpt = enemyActor.OwnerId |> tryGetOwner scene
            match isAlly player ownerOpt with
            | false -> 0.0
            | true ->
                getActorPower enemyActor
        )
    let decorationCoefficient = tileDecorations |> List.sumBy getDecorationHelp
    let effectsCoefficient = tileEffects |> List.sumBy getEffectHelp
    let tilesCoefficient = tiles |> List.sumBy getTileHelp
    normalize ((actorCoefficient + decorationCoefficient) * massActorShare + (tilesCoefficient + effectsCoefficient) * (1.0 - massActorShare))

let private getCellValue (x: int, y: int, t: CellNeuronType) (scene: Scene) =
    scene
    |> tryGetCurrentActorAndHisOwner
    |> Option.map (fun actorAndOwner ->
        let realX, realY = x * 4, y * 4
        let tilesInfo = (
            scene.Tiles |> Seq.filter (fun t -> t.X >= realX && t.X < realX + 4 && t.Y >= realY && t.Y < realY + 4) |> Seq.toList,
            scene.Actors |> Seq.filter (fun t -> t.X >= realX && t.X < realX + 4 && t.Y >= realY && t.Y < realY + 4) |> Seq.toList,
            scene.Decorations |> Seq.filter (fun t -> t.X >= realX && t.X < realX + 4 && t.Y >= realY && t.Y < realY + 4) |> Seq.toList,
            scene.Effects|> Seq.filter (fun t -> t.X >= realX && t.X < realX + 4 && t.Y >= realY && t.Y < realY + 4) |> Seq.toList
        )
        let tiles, _, _, _ = tilesInfo
        match t with
            | Dangerous -> calculateSectorDangerousParam scene tilesInfo actorAndOwner
            | Friendly -> calculateSectorFriendlyParam scene tilesInfo actorAndOwner
            | Unbearable -> tiles |> List.averageBy calculateCellUnbearableParam
            | Active -> tiles |> List.averageBy (fun tile -> match tile.TempActorId.IsSome with | true -> 1.0 | false -> 0.0))
    |> Option.defaultValue 0.0

let private getCellNeuron (scene: Scene option) (x: int, y: int) =
    [ Dangerous; Friendly; Unbearable; Active ]
    |> List.map (fun t ->
        let modifier = match t with
                       | Dangerous -> "d"
                       | Friendly -> "f"
                       | Unbearable -> "u"
                       | Active -> "a"
        {
            Name = sprintf "c%i%i%s" x y modifier
            Value = scene |> Option.map (getCellValue (x, y, t)) |> Option.defaultValue 0.0
        })
            
let getCellsInputNeurons (scene: Scene option) =
    [0..27]
    |> List.map (fun p -> (p % 7, p / 7))
    |> List.collect (fun c -> c |> getCellNeuron scene)
