module ProjectArena.Bot.Helpers.Neural.CellsHelper
open System
open ProjectArena.Bot.Models.Neural
open ProjectArena.Bot.Models.States
open ProjectArena.Bot.Models.Dtos
open ProjectArena.Bot.Helpers.SceneHelper
open System.Diagnostics

let actorShare = 0.6
let massActorShare = 0.8

let private calculateCellActiveParam (tile: TileDto) (tileEffects: SpecEffectDto list) =
    if tile.TempActorId.IsSome || (tileEffects |> List.isEmpty) then
        1.0
    else
        0.0

let private calculateCellUnbearableParam (tile: TileDto) =
    if tile.Unbearable || tile.TempActorId.IsSome then
        1.0
    else
        0.0

let private calculateCellFriendlyParam
    (scene: Scene)
    (tile: TileDto, tileActor: ActorDto option, tileDecoration: ActiveDecorationDto option, tileEffects: SpecEffectDto list)
    (actor: ActorDto, player: PlayerDto) =

    let actorCoefficient =
        tileActor
        |> Option.map (fun enemyActor ->
            let ownerOpt = enemyActor.OwnerId |> tryGetOwner scene
            match isAlly player ownerOpt with
            | false -> 0.0
            | true ->
                getActorPower enemyActor
        )
        |> Option.defaultValue 0.0
    let decorationCoefficient =
        tileDecoration
        |> Option.map getDecorationHelp
        |> Option.defaultValue 0.0
    let effectsCoefficient = match tileEffects.Length with
                             | 0 -> 0.0
                             | _ -> 
                                tileEffects
                                |> List.map getEffectHelp
                                |> List.append [ getTileHelp tile ]
                                |> List.average
    (actorCoefficient + decorationCoefficient) * actorShare + effectsCoefficient * (1.0 - actorShare)

let private calculateCellDangerousParam
    (scene: Scene)
    (tile: TileDto, tileActor: ActorDto option, tileDecoration: ActiveDecorationDto option, tileEffects: SpecEffectDto list)
    (actor: ActorDto, player: PlayerDto) =

    let actorCoefficient =
        tileActor
        |> Option.map (fun enemyActor ->
            let ownerOpt = enemyActor.OwnerId |> tryGetOwner scene
            match isAlly player ownerOpt with
            | true -> 0.0
            | false ->
                Math.Min (1.0, getActorPower enemyActor / getActorPower actor * strengthDifferenceCoefficient)
        )
        |> Option.defaultValue 0.0
    let decorationCoefficient =
        tileDecoration
        |> Option.map getDecorationDanger
        |> Option.defaultValue 0.0
    let effectsCoefficient = match tileEffects.Length with
                             | 0 -> 0.0
                             | _ -> 
                                tileEffects
                                |> List.map getEffectDanger
                                |> List.append [ getTileDanger tile ]
                                |> List.average
    (actorCoefficient + decorationCoefficient) * actorShare + effectsCoefficient * (1.0 - actorShare)

let private getParticularCellValue (x: int, y: int, t: CellNeuronType) (scene: Scene) (actorAndPlayer) =
    let tileInfo = (
        scene.Tiles |> Seq.find (fun v -> v.X = x && v.Y = y),
        scene.Actors |> Seq.tryFind (fun a -> a.X = x && a.Y = y),
        scene.Decorations |> Seq.tryFind (fun d -> d.X = x && d.Y = y),
        scene.Effects |> Seq.filter (fun e -> e.X = x && e.Y = y) |> Seq.toList)
    let tile, _, _, effects = tileInfo
    match t with
    | Dangerous -> calculateCellDangerousParam scene tileInfo actorAndPlayer
    | Friendly -> calculateCellFriendlyParam scene tileInfo actorAndPlayer
    | Unbearable -> calculateCellUnbearableParam tile
    | Active -> calculateCellActiveParam tile effects

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

let private getMagnifyingCellValue (x: int, y: int, t: CellNeuronType) (actorAndOwner) (scene: Scene) =
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
        | Active -> tiles |> List.averageBy (fun tile -> match tile.TempActorId.IsSome with | true -> 1.0 | false -> 0.0)

let private getCommandCellValue (shiftX: int, shiftY: int) (x: int, y: int, t: CellNeuronType) (actorAndOwner) (scene: Scene) =
    getParticularCellValue (shiftX + x, shiftY + y, t) scene actorAndOwner

let private getCellValue (magnified: (int * int) option) info (scene: Scene) =
    scene |> tryGetCurrentActorAndHisOwner
    |> Option.map (fun actorAndOwner ->
        match magnified with
        | None -> scene |> getMagnifyingCellValue info actorAndOwner
        | Some v -> scene |> getCommandCellValue v info actorAndOwner)
    |> Option.defaultValue 0.0

let private getCellNeuron (magnified: (int * int) option) (scene: Scene option) (x: int, y: int) =
    [ Dangerous; Friendly; Unbearable; Active ]
    |> List.map (fun t ->
        let modifier = match t with
                       | Dangerous -> "d"
                       | Friendly -> "f"
                       | Unbearable -> "u"
                       | Active -> "a"
        {
            Name = sprintf "c%i%i%s" x y modifier
            Value = scene |> Option.map (getCellValue magnified (x, y, t)) |> Option.defaultValue 0.0
        })
            
let getMagnifyingCellsInputNeurons (scene: Scene option) =
    [0..27]
    |> List.map (fun p -> (p % 7, p / 7))
    |> List.collect (fun c -> c |> getCellNeuron None scene)

let getCommandCellsInputNeurons (shift: int * int) (scene: Scene option) =
    [0..15]
    |> List.map (fun p -> (p % 4, p / 4))
    |> List.collect (fun c -> c |> getCellNeuron (Some shift) scene)
