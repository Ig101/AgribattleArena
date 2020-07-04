module ProjectArena.Bot.Processors.SceneProcessor
open ProjectArena.Bot.Domain.BotMongoContext.Entities
open ProjectArena.Bot.Domain.GameConnection.GameApi
open ProjectArena.Bot.Models.States
open ProjectArena.Bot.Models.Dtos
open ProjectArena.Bot.Models.Configuration
open ProjectArena.Bot.Processors.NeuralProcessor
open ProjectArena.Infrastructure.Enums
open Microsoft.Extensions.Logging
open ProjectArena.Bot.Functors

let private findActor (actors: ActorDto seq) (idOpt: int option) =
    idOpt |> Option.bind (fun id -> actors |> Seq.tryFind (fun a -> a.Id = id))

let private findDecoration (decorations: ActiveDecorationDto seq) (idOpt: int option) =
    idOpt |> Option.bind (fun id -> decorations |> Seq.tryFind (fun a -> a.Id = id))

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
        TurnTime = synchronizer.TurnTime
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
        |> Seq.append (scene.Actors |> Seq.filter (actorsFilter synchronizer.ChangedActors synchronizer.DeletedActors)) |> Seq.toList
    let decorations =
        synchronizer.ChangedDecorations
        |> Seq.append (scene.Decorations |> Seq.filter (decorationsFilter synchronizer.ChangedDecorations synchronizer.DeletedDecorations)) |> Seq.toList
    let effects =
        synchronizer.ChangedEffects
        |> Seq.append (scene.Effects |> Seq.filter (effectsFilter synchronizer.ChangedEffects synchronizer.DeletedEffects)) |> Seq.toList
    let tiles =
        synchronizer.ChangedTiles
        |> Seq.append (scene.Tiles |> Seq.filter (tilesFilter synchronizer.ChangedTiles)) |> Seq.toList
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
        TurnTime = synchronizer.TurnTime
    }

let private createScene (configuration: Configuration) (synchronizer: SynchronizerDto) = async {
    match synchronizer.Version with
    | 1 ->
        return Some (generateSceneFromSynchronizer synchronizer)
    | _ ->
        let! realSynchronizer = getSceneSynchronizer configuration.Logger configuration.User.AuthCookie configuration.ApiHost synchronizer.Id
        return realSynchronizer |> Option.map generateSceneFromSynchronizer
}

let private mergeScene (configuration: Configuration) (synchronizer: SynchronizerDto) (scene: Scene) = async {
    match synchronizer.Version - scene.Version with
    | 1 ->
        return Some (mergeSceneWithSynchronizer scene synchronizer)
    | diff when diff > 1 ->
        let! realSynchronizer = getSceneSynchronizer configuration.Logger configuration.User.AuthCookie configuration.ApiHost synchronizer.Id
        return realSynchronizer |> Option.map generateSceneFromSynchronizer
    | _ ->
        return None
}

let private tryMergeScene (configuration: Configuration) (synchronizer: SynchronizerDto) (sceneOpt: Scene option) =
    match sceneOpt with
    | Some scene -> mergeScene configuration synchronizer scene
    | None -> createScene configuration synchronizer

let private chooseMeaningfulActionsOnly (message: IncomingSynchronizationMessage) (scene: Scene) =
    match message.Synchronizer.ActorId with
    | None -> Some scene
    | Some actingActor ->
        match message.Synchronizer.TempActor with
        | None -> Some scene
        | Some tempActor ->
            let actorOpt = message.Synchronizer.ChangedActors |> Seq.tryFind (fun a -> a.Id = tempActor)
            match actorOpt with
            | None -> None
            | Some actor ->
                if message.Action = Wait || (actingActor = tempActor && ((not actor.CanAct && not actor.CanMove))) then
                    None
                else 
                    Some scene

let private tryGetActingModel (userId: string) (scene: Scene) =
    match scene.TempActor with
    | None -> None
    | Some actor ->
        let ownerOpt =
            actor.Owner
        match ownerOpt with
        | Some owner when owner.UserId = userId -> Some scene
        | _ -> None

let private leaveIfTooLong (configuration: Configuration) (message: IncomingSynchronizationMessage) (scene: Scene) =
    let currentPlayer = scene.Players |> Seq.filter (fun p -> p.UserId = configuration.User.UserId) |> Seq.head
    match currentPlayer.Status with
    | PlayerStatus.Playing when scene.RoundsPassed < configuration.Learning.TimeTillSurrender || message.Action = EndGame -> Some scene
    | PlayerStatus.Playing ->
        leaveBattle configuration.Logger configuration.User.AuthCookie configuration.ApiHost scene.Id |> Async.map (ignore) |> Async.Start
        configuration.Worker.Leave message.Synchronizer
        None
    | _ -> None

let private chooseModel (userId: string) (model, spareModel) (scene: Scene) =
    let index = scene.Players |> Seq.filter (fun p -> p.UserId = userId) |> Seq.findIndex (fun p -> p = scene.TempActor.Value.Owner.Value)
    match index with
    | 0 -> (model, scene)
    | _ -> (spareModel, scene)

let sceneMessageProcessor (configuration: Configuration) (model: NeuralModelContainer, spareModel: NeuralModelContainer) (sceneOpt: Scene option) (message: IncomingSynchronizationMessage ) = async {
    let! newSceneOpt =
        sceneOpt
        |> tryMergeScene configuration message.Synchronizer
    let actionModelOpt = 
        newSceneOpt
        |> Option.bind (chooseMeaningfulActionsOnly message)
        |> Option.bind (tryGetActingModel configuration.User.UserId)
        |> Option.bind (leaveIfTooLong configuration message)
        |> Option.map (chooseModel configuration.User.UserId (model, spareModel))
    match actionModelOpt with
    | Some actionModel -> do! actOnScene configuration actionModel
    | None -> ()
    match newSceneOpt with
    | None -> return sceneOpt
    | _ -> return newSceneOpt
}

let tryCalculatePerformance (configuration: Configuration) (sceneOpt: Scene option): float =
    let enrichSceneWithPlayer (scene: Scene) =
        (scene, scene.Players |> Seq.find (fun p -> p.UserId = configuration.User.UserId))
    let calculateVictoryPerformance (scene: Scene, player: PlayerDto) (performance: float) =
        let newPerformance = match player.Status with
                             | PlayerStatus.Victorious -> performance + configuration.Learning.VictoryPerformanceCoefficient
                             | _ -> performance
        newPerformance
    let calculatePlayerPowerPerformance (scene: Scene, player: PlayerDto) (performance: float) =
        let newPerformance =
            scene.Actors
            |> Seq.toList
            |> List.filter (fun a -> a.Owner.IsSome && a.Owner.Value = player)
            |> List.fold (fun result a -> result + float(a.Health |> Option.defaultValue 100.0f) / float(a.MaxHealth |> Option.defaultValue 100)) 0.0
            |> fun v -> performance + configuration.Learning.PlayerPowerPerformanceCoefficient * (v / 3.0)
        newPerformance
    let calculateEnemyPowerPerformance (scene: Scene, player: PlayerDto) (performance: float) =
        let opponent = scene.Players |> Seq.toList |> List.find (fun p -> p.Id <> player.Id)
        let newPerformance =
            scene.Actors
            |> Seq.toList
            |> List.filter (fun a -> a.Owner.IsSome && a.Owner.Value = opponent)
            |> List.fold (fun result a -> result + float(a.Health |> Option.defaultValue 100.0f) / float(a.MaxHealth |> Option.defaultValue 100)) 0.0
            |> fun v -> performance + configuration.Learning.EnemyPowerPerformanceCoefficient * (1.0 - v / 3.0)
        newPerformance
    match sceneOpt with
    | Some scene ->
        let scenePlayer = enrichSceneWithPlayer scene
        0.0 |> calculateVictoryPerformance scenePlayer |> calculatePlayerPowerPerformance scenePlayer |> calculateEnemyPowerPerformance scenePlayer
    | None -> 0.0