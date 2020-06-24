module ProjectArena.Bot.Helpers.MappingHelper
open ProjectArena.Bot.Models.Dtos

let private mapPlayer (player: ProjectArena.Infrastructure.Models.Battle.Synchronization.PlayerDto) =
    {
        PlayerDto.Id = player.Id
        UserId = player.UserId
        Team = player.Team |> Option.ofNullable
        KeyActorsSync = player.KeyActorsSync
        TurnsSkipped = player.TurnsSkipped
        Status = player.Status
    }

let private mapSkill (skill: ProjectArena.Infrastructure.Models.Battle.Synchronization.SkillDto) =
    {
        SkillDto.Id = skill.Id
        Range = skill.Range
        NativeId = skill.NativeId
        Visualization = skill.Visualization
        Cd = skill.Cd
        Cost = skill.Cost
        PreparationTime = skill.PreparationTime
        AvailableTargets = {
            TargetsDto.Allies = skill.AvailableTargets.Allies
            Self = skill.AvailableTargets.Self
            NotAllies = skill.AvailableTargets.NotAllies
            Bearable = skill.AvailableTargets.Bearable
            Unbearable = skill.AvailableTargets.Unbearable
            Decorations = skill.AvailableTargets.Decorations
        }
        OnlyVisibleTargets = skill.OnlyVisibleTargets
    }

let private mapBuff (buff: ProjectArena.Infrastructure.Models.Battle.Synchronization.BuffDto) =
    {
        BuffDto.Id = buff.Id
        NativeId = buff.NativeId
        Duration = buff.Duration |> Option.ofNullable
    }

let private mapActor (actor: ProjectArena.Infrastructure.Models.Battle.Synchronization.ActorDto) =
    {
        ActorDto.Id = actor.Id
        ExternalId = actor.ExternalId |> Option.ofNullable
        NativeId = actor.NativeId
        Visualization = actor.Visualization
        AttackingSkill = actor.AttackingSkill |> Option.ofObj |> Option.map mapSkill
        Skills = actor.Skills |> Seq.map mapSkill |> Seq.toList
        Buffs = actor.Buffs |> Seq.map mapBuff |> Seq.toList
        InitiativePosition = actor.InitiativePosition
        Health = actor.Health |> Option.ofNullable
        OwnerId = actor.OwnerId |> Option.ofObj
        X = actor.X
        Y = actor.Y
        Z = actor.Z
        MaxHealth = actor.MaxHealth |> Option.ofNullable
        ActionPoints = actor.ActionPoints
        Initiative = actor.Initiative
        CanMove = actor.CanMove
        CanAct = actor.CanAct
    }

let private mapDecoration (decoration: ProjectArena.Infrastructure.Models.Battle.Synchronization.ActiveDecorationDto) =
    {
        ActiveDecorationDto.Id = decoration.Id
        NativeId = decoration.NativeId
        Visualization = decoration.Visualization
        InitiativePosition = decoration.InitiativePosition
        Health = decoration.Health |> Option.ofNullable
        OwnerId = decoration.OwnerId |> Option.ofObj
        IsAlive = decoration.IsAlive
        X = decoration.X
        Y = decoration.Y
        Z = decoration.Z
        MaxHealth = decoration.MaxHealth |> Option.ofNullable
    }

let private mapEffect (effect: ProjectArena.Infrastructure.Models.Battle.Synchronization.SpecEffectDto) =
    {
        SpecEffectDto.Id = effect.Id
        OwnerId = effect.OwnerId |> Option.ofObj
        IsAlive = effect.IsAlive
        X = effect.X
        Y = effect.Y
        Z = effect.Z
        Duration = effect.Duration |> Option.ofNullable
        NativeId = effect.NativeId
        Visualization = effect.Visualization
    }

let private mapTile (tile: ProjectArena.Infrastructure.Models.Battle.Synchronization.TileDto) =
    {
        TileDto.X = tile.X
        Y = tile.Y
        TempActorId = tile.TempActorId |> Option.ofNullable
        Height = tile.Height
        NativeId = tile.NativeId
        OwnerId = tile.OwnerId |> Option.ofObj
        Unbearable = tile.Unbearable
    }

let mapSynchronizer (synchronizer: ProjectArena.Infrastructure.Models.Battle.Synchronization.SynchronizerDto) =
    {
        SynchronizerDto.Id = synchronizer.Id
        Version = synchronizer.Version
        RoundsPassed = synchronizer.RoundsPassed
        ActorId = synchronizer.ActorId |> Option.ofNullable
        SkillActionId = synchronizer.SkillActionId |> Option.ofNullable
        TargetX = synchronizer.TargetX |> Option.ofNullable
        TargetY = synchronizer.TargetY |> Option.ofNullable
        TurnTime = synchronizer.TurnTime
        TempActor = synchronizer.TempActor |> Option.ofNullable
        TempDecoration = synchronizer.TempDecoration |> Option.ofNullable
        Players = synchronizer.Players |> Seq.map mapPlayer |> Seq.toList
        ChangedActors = synchronizer.ChangedActors |> Seq.map mapActor |> Seq.toList
        ChangedDecorations = synchronizer.ChangedDecorations |> Seq.map mapDecoration |> Seq.toList
        ChangedEffects = synchronizer.ChangedEffects |> Seq.map mapEffect |> Seq.toList
        DeletedActors = synchronizer.DeletedActors
        DeletedDecorations = synchronizer.DeletedDecorations
        DeletedEffects = synchronizer.DeletedEffects
        ChangedTiles = synchronizer.ChangedTiles |> Seq.map mapTile |> Seq.toList
        TilesetWidth = synchronizer.TilesetWidth
        TilesetHeight = synchronizer.TilesetHeight
    }