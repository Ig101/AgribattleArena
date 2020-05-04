using System.Collections.Generic;
using System.Linq;
using ProjectArena.Domain.BattleService.Models;
using ProjectArena.Domain.QueueService.Models;
using ProjectArena.Engine.ForExternalUse;
using ProjectArena.Engine.ForExternalUse.EngineHelper;
using ProjectArena.Engine.ForExternalUse.Synchronization;
using ProjectArena.Infrastructure.Enums;
using ProjectArena.Infrastructure.Models.Battle.Synchronization;

namespace ProjectArena.Domain.BattleService.Helpers
{
    public static class BattleHelper
    {
        public static string GetBattleActionMethodName(Engine.Helpers.Action? action)
        {
            return "Battle" + (action == null ? "Info" : action.ToString());
        }

        private static TagSynergyDto MapTagSynergy(Engine.Helpers.TagSynergy model)
        {
            return new TagSynergyDto()
            {
                SelfTag = model.SelfTag,
                TargetTag = model.TargetTag,
                Mod = model.Mod
            };
        }

        public static IDictionary<GameMode, SceneModeQueue> GetNewModeQueue()
        {
            return new Dictionary<GameMode, SceneModeQueue>()
            {
                {
                    GameMode.Patrol, new SceneModeQueue()
                    {
                        Queue = new HashSet<UserInQueue>(),
                        Mode = new SceneMode()
                        {
                            Generator = EngineHelper.CreateDuelSceneGenerator(),
                            VarManager = EngineHelper.CreateVarManager(80, 20, 3, 8, 5, 0.1f, 0.1f, 0.1f),
                            BattleResultProcessor = BattleResultProcessors.ProcessMainDuelBattleResult,
                            MaxPlayers = 2
                        }
                    }
                }
            };
        }

        private static ActorDto MapActor(Engine.ForExternalUse.Synchronization.ObjectInterfaces.IActor actor)
        {
            return new ActorDto()
            {
                ActionPoints = actor.ActionPoints,
                AttackingSkill = new SkillDto()
                {
                    Cd = actor.AttackingSkill.Cd,
                    Cost = actor.AttackingSkill.Cost,
                    Id = actor.AttackingSkill.Id,
                    NativeId = actor.AttackingSkill.NativeId,
                    PreparationTime = actor.AttackingSkill.PreparationTime,
                    Range = actor.AttackingSkill.Range,
                    MeleeOnly = actor.AttackingSkill.MeleeOnly
                },
                Buffs = actor.Buffs.Select(k => new BuffDto()
                {
                    Duration = k.Duration,
                    Id = k.Id,
                    NativeId = k.NativeId
                }),
                NativeId = actor.NativeId,
                Id = actor.Id,
                ExternalId = actor.ExternalId,
                Initiative = actor.Initiative,
                InitiativePosition = actor.InitiativePosition,
                MaxHealth = actor.MaxHealth,
                Health = actor.Health,
                OwnerId = actor.OwnerId,
                Skills = actor.Skills.Select(k => new SkillDto()
                {
                    Cd = k.Cd,
                    Cost = k.Cost,
                    Id = k.Id,
                    NativeId = k.NativeId,
                    PreparationTime = k.PreparationTime,
                    Range = k.Range,
                    MeleeOnly = k.MeleeOnly
                }),
                X = actor.X,
                Y = actor.Y,
                Z = actor.Z,
                CanMove = actor.CanMove,
                CanAct = actor.CanAct
            };
        }

        private static ActiveDecorationDto MapDecoration(Engine.ForExternalUse.Synchronization.ObjectInterfaces.IActiveDecoration decoration)
        {
            return new ActiveDecorationDto()
            {
                Armor = decoration.Armor.Select(x => MapTagSynergy(x)),
                Health = decoration.Health,
                Id = decoration.Id,
                InitiativePosition = decoration.InitiativePosition,
                IsAlive = decoration.IsAlive,
                MaxHealth = decoration.MaxHealth,
                Mod = decoration.Mod,
                NativeId = decoration.NativeId,
                OwnerId = decoration.OwnerId,
                X = decoration.X,
                Y = decoration.Y,
                Z = decoration.Z
            };
        }

        private static SpecEffectDto MapEffect(Engine.ForExternalUse.Synchronization.ObjectInterfaces.ISpecEffect effect)
        {
            return new SpecEffectDto()
            {
                Duration = effect.Duration,
                Id = effect.Id,
                IsAlive = effect.IsAlive,
                Mod = effect.Mod,
                NativeId = effect.NativeId,
                OwnerId = effect.OwnerId,
                X = effect.X,
                Y = effect.Y,
                Z = effect.Z
            };
        }

        private static TileDto MapTile(Engine.ForExternalUse.Synchronization.ObjectInterfaces.ITile tile)
        {
            return new TileDto()
            {
                Height = tile.Height,
                NativeId = tile.NativeId,
                OwnerId = tile.OwnerId,
                TempActorId = tile.TempActorId,
                X = tile.X,
                Y = tile.Y,
                Unbearable = tile.Unbearable
            };
        }

        private static SynchronizerDto MapSynchronizer(ISynchronizer oldSynchronizer)
        {
            var tileSet = oldSynchronizer.TileSet;
            return new SynchronizerDto()
            {
                ChangedActors = oldSynchronizer.ChangedActors.Select(x => MapActor(x)),
                DeletedActors = oldSynchronizer.DeletedActors,
                ChangedDecorations = oldSynchronizer.ChangedDecorations.Select(x => MapDecoration(x)),
                DeletedDecorations = oldSynchronizer.DeletedDecorations,
                ChangedEffects = oldSynchronizer.ChangedEffects.Select(x => MapEffect(x)),
                DeletedEffects = oldSynchronizer.DeletedEffects,
                Players = oldSynchronizer.Players.Select(x => new PlayerDto()
                {
                    Id = x.Id,
                    KeyActorsSync = x.KeyActorsSync,
                    Status = (PlayerStatus)(int)x.Status,
                    Team = x.Team,
                    TurnsSkipped = x.TurnsSkipped
                }),
                ChangedTiles = oldSynchronizer.ChangedTiles.Select(x => MapTile(x)),
                TempActor = oldSynchronizer.TempActor,
                TempDecoration = oldSynchronizer.TempDecoration,
                TilesetHeight = tileSet.GetLength(1),
                TilesetWidth = tileSet.GetLength(0)
            };
        }

        public static SynchronizerDto MapSynchronizer(ISyncEventArgs syncEventArgs)
        {
            var synchronizer = MapSynchronizer(syncEventArgs.SyncInfo);
            synchronizer.TargetX = syncEventArgs.TargetX;
            synchronizer.TargetY = syncEventArgs.TargetY;
            synchronizer.SkillActionId = syncEventArgs.SkillActionId;
            synchronizer.ActorId = syncEventArgs.ActorId;
            synchronizer.Version = syncEventArgs.Version;
            synchronizer.TurnTime = syncEventArgs.Scene.RemainedTurnTime;
            return synchronizer;
        }

        public static SynchronizerDto MapSynchronizer(ISynchronizer oldSynchronizer, IScene scene)
        {
            var synchronizer = MapSynchronizer(oldSynchronizer);
            synchronizer.Version = scene.Version;
            synchronizer.TurnTime = scene.RemainedTurnTime;
            return synchronizer;
        }

        public static SynchronizerDto GetFullSynchronizationData(IScene scene)
        {
            var synchronizer = scene.GetFullSynchronizationData();
            return MapSynchronizer(synchronizer, scene);
        }
    }
}