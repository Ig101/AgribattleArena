module ProjectArena.Bot.Processors.SceneProcessor
open ProjectArena.Bot.Domain.BotMongoContext.Entities
open ProjectArena.Bot.Domain.GameConnection.GameApi
open ProjectArena.Bot.Models.States
open ProjectArena.Bot.Models.Dtos
open ProjectArena.Bot.Models.Configuration
open ProjectArena.Bot.Processors.NeuralProcessor

let private findActor (actors: ActorDto seq) (idOpt: int option) =
    idOpt |> Option.map (fun id -> actors |> Seq.find (fun a -> a.Id = id))

let private findDecoration (decorations: ActiveDecorationDto seq) (idOpt: int option) =
    idOpt |> Option.map (fun id -> decorations |> Seq.find (fun a -> a.Id = id))

let private generateSceneFromSynchronizer (synchronizer: SynchronizerDto) =
    {
        Id = synchronizer.Id
        Version = synchronizer.Version
        RoundsPassed = synchronizer.RoundsPassed
        TempActor = findActor synchronizer.ChangedActors synchronizer.TempActor
        TempDecoration = findDecoration synchronizer.ChangedDecorations synchronizer.TempDecoration
        Players = synchronizer.Players
        Actors = synchronizer.ChangedActors
        Decorations = synchronizer.ChangedDecorations
        Effects = synchronizer.ChangedEffects
        Tiles = synchronizer.ChangedTiles
        TilesetWidth = synchronizer.TilesetWidth
        TilesetHeight = synchronizer.TilesetHeight
    }

let private mergeSceneWithSynchronizer (scene: Scene) (synchronizer: SynchronizerDto) =
    let actorsFilter (changedActors: ActorDto seq) (deletedActors: int seq) (actor: ActorDto) =
        not (changedActors |> Seq.map (fun a -> a.Id) |> Seq.append deletedActors |> Seq.contains actor.Id)
    let decorationsFilter (changedDecorations: ActiveDecorationDto seq) (deletedDecorations: int seq) (decoration: ActiveDecorationDto) =
        not (changedDecorations |> Seq.map (fun d -> d.Id) |> Seq.append deletedDecorations |> Seq.contains decoration.Id)
    let effectsFilter (changedEffects: SpecEffectDto seq) (deletedEffects: int seq) (effect: SpecEffectDto) =
        not (changedEffects |> Seq.map (fun e -> e.Id) |> Seq.append deletedEffects |> Seq.contains effect.Id)
    let tilesFilter (changedTiles: TileDto seq) (tile: TileDto) =
        not (changedTiles |> Seq.exists (fun t -> t.X = tile.X && t.Y = tile.Y))

    let actors =
        synchronizer.ChangedActors
        |> Seq.append (scene.Actors |> Seq.filter (actorsFilter synchronizer.ChangedActors synchronizer.DeletedActors))
    let decorations =
        synchronizer.ChangedDecorations
        |> Seq.append (scene.Decorations |> Seq.filter (decorationsFilter synchronizer.ChangedDecorations synchronizer.DeletedDecorations))
    let effects =
        synchronizer.ChangedEffects
        |> Seq.append (scene.Effects |> Seq.filter (effectsFilter synchronizer.ChangedEffects synchronizer.DeletedEffects))
    let tiles =
        synchronizer.ChangedTiles
        |> Seq.append (scene.Tiles |> Seq.filter (tilesFilter synchronizer.ChangedTiles))
    {
        Id = synchronizer.Id
        Version = synchronizer.Version
        RoundsPassed = synchronizer.RoundsPassed
        TempActor = findActor actors synchronizer.TempActor
        TempDecoration = findDecoration decorations synchronizer.TempDecoration
        Players = synchronizer.Players
        Actors = actors
        Decorations = decorations
        Effects = effects
        Tiles = tiles
        TilesetWidth = synchronizer.TilesetWidth
        TilesetHeight = synchronizer.TilesetHeight
    }

let private createScene (apiHost: string) (synchronizer: SynchronizerDto) = async {
    match synchronizer.Version with
    | 1 ->
        return Some (generateSceneFromSynchronizer synchronizer)
    | _ ->
        let! realSynchronizer = getSceneSynchronizer apiHost synchronizer.Id
        return realSynchronizer |> Option.map generateSceneFromSynchronizer
}

let private mergeScene (apiHost: string) (synchronizer: SynchronizerDto) (scene: Scene) = async {
    match synchronizer.Version - scene.Version with
    | 1 ->
        return Some (mergeSceneWithSynchronizer scene synchronizer)
    | diff when diff > 1 ->
        let! realSynchronizer = getSceneSynchronizer apiHost synchronizer.Id
        return realSynchronizer |> Option.map generateSceneFromSynchronizer
    | _ ->
        return None
}

let private tryMergeScene (apiHost: string) (synchronizer: SynchronizerDto) (sceneOpt: Scene option) =
    match sceneOpt with
    | Some scene -> mergeScene apiHost synchronizer scene
    | None -> createScene apiHost synchronizer

let private chooseModel (userId: string) (model: NeuralModel, spareModel: NeuralModel) (scene: Scene) (playerId: string) =
    let index = scene.Players |> Seq.filter (fun p -> p.UserId = userId) |> Seq.findIndex (fun p -> p.Id = playerId)
    match index with
    | 0 -> (model, scene)
    | _ -> (spareModel, scene)

let private tryGetActingModel (userId: string) (model: NeuralModel, spareModel: NeuralModel) (scene: Scene) =
    match scene.TempActor with
    | None -> None
    | Some actor ->
        let ownerOpt =
            actor.OwnerId
            |> Option.map (fun ownerId -> scene.Players |> Seq.find (fun p -> p.Id = ownerId))
        match ownerOpt with
        | Some owner when owner.UserId = userId -> Some (owner.Id |> chooseModel userId (model, spareModel) scene)
        | _ -> None



let sceneMessageProcessor (configuration: Configuration) (model: NeuralModel, spareModel: NeuralModel) (sceneOpt: Scene option) (message: IncomingSynchronizationMessage ) = async {
    let! newSceneOpt =
        sceneOpt
        |> tryMergeScene configuration.ApiHost message.Synchronizer;
    
    newSceneOpt
    |> Option.bind (tryGetActingModel configuration.UserId (model, spareModel))
    |> Option.map (actOnScene configuration.Hub)
    |> ignore
    match newSceneOpt with
    | None -> return sceneOpt
    | _ -> return newSceneOpt
}

let tryCalculatePerformance (sceneOpt: Scene option) =
    let calculatePerformance (scene: Scene) =
        // TODO Performance calculation
        1.0
    match sceneOpt with
    | Some scene -> calculatePerformance scene
    | None -> 0.0