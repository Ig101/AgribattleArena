using System;
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
        public const int DefaultStrength = 25;

        public const int DefaultWillpower = 25;

        public const int DefaultConstitution = 25;

        public const int DefaultSpeed = 25;

        public const int VictoryReward = 5;

        public const int DefeatReward = 2;

        public static string GetBattleActionMethodName(Engine.Helpers.SceneAction? action)
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
                            VarManager = EngineHelper.CreateVarManager(40, 20, 3, 8, 4, 0.05f, 0.05f, 0.03f, 0.4f),
                            BattleResultProcessor = BattleResultProcessors.ProcessMainDuelBattleResult,
                            MaxPlayers = 2,
                            TimeTillBot = 20,
                            AllowMultiEnqueue = false
                        }
                    }
                },
                {
                    GameMode.BotLearning, new SceneModeQueue()
                    {
                        Queue = new HashSet<UserInQueue>(),
                        Mode = new SceneMode()
                        {
                            Generator = EngineHelper.CreateDuelSceneGenerator(),
                            VarManager = EngineHelper.CreateVarManager(60, 10, 3, 8, 4, 0.05f, 0.05f, 0.03f, 0.4f),
                            BattleResultProcessor = BattleResultProcessors.ProcessMainDuelBattleResult,
                            MaxPlayers = 2,
                            TimeTillBot = 5,
                            AllowMultiEnqueue = true
                        }
                    }
                }
            };
        }

        private static ActorDto MapActor(Engine.ForExternalUse.Synchronization.ObjectInterfaces.IActor actor, bool ally)
        {
            return new ActorDto()
            {
                ActionPoints = actor.ActionPoints,
                AttackingSkill = ally || actor.AttackingSkill.Revealed ? new SkillDto()
                {
                    Cd = actor.AttackingSkill.Cd,
                    Cost = actor.AttackingSkill.Cost,
                    Id = actor.AttackingSkill.Id,
                    NativeId = actor.AttackingSkill.NativeId,
                    PreparationTime = actor.AttackingSkill.PreparationTime,
                    Range = actor.AttackingSkill.Range,
                    OnlyVisibleTargets = actor.AttackingSkill.OnlyVisibleTargets,
                    AvailableTargets = new TargetsDto()
                    {
                        Allies = actor.AttackingSkill.AvailableTargets.Allies,
                        Self = actor.AttackingSkill.AvailableTargets.Self,
                        NotAllies = actor.AttackingSkill.AvailableTargets.NotAllies,
                        Bearable = actor.AttackingSkill.AvailableTargets.Bearable,
                        Unbearable = actor.AttackingSkill.AvailableTargets.Unbearable,
                        Decorations = actor.AttackingSkill.AvailableTargets.Decorations
                    }
                }
                : null,
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
                MaxHealth = actor.HealthRevealed || ally ? actor.MaxHealth : (int?)null,
                Health = actor.HealthRevealed || ally ? actor.Health : (float?)null,
                OwnerId = actor.OwnerId,
                Skills = actor.Skills.Where(k => ally || k.Revealed).Select(k => new SkillDto()
                {
                    Cd = k.Cd,
                    Cost = k.Cost,
                    Id = k.Id,
                    NativeId = k.NativeId,
                    PreparationTime = k.PreparationTime,
                    Range = k.Range,
                    OnlyVisibleTargets = k.OnlyVisibleTargets,
                    AvailableTargets = new TargetsDto()
                    {
                        Allies = k.AvailableTargets.Allies,
                        Self = k.AvailableTargets.Self,
                        NotAllies = k.AvailableTargets.NotAllies,
                        Bearable = k.AvailableTargets.Bearable,
                        Unbearable = k.AvailableTargets.Unbearable,
                        Decorations = k.AvailableTargets.Decorations
                    }
                }),
                X = actor.X,
                Y = actor.Y,
                Z = actor.Z,
                CanMove = actor.CanMove,
                CanAct = actor.CanAct
            };
        }

        private static ActiveDecorationDto MapDecoration(Engine.ForExternalUse.Synchronization.ObjectInterfaces.IActiveDecoration decoration, bool ally)
        {
            return new ActiveDecorationDto()
            {
                Armor = decoration.Armor.Select(x => MapTagSynergy(x)),
                Health = decoration.HealthRevealed || ally ? decoration.Health : (float?)null,
                Id = decoration.Id,
                InitiativePosition = decoration.InitiativePosition,
                IsAlive = decoration.IsAlive,
                MaxHealth = decoration.HealthRevealed || ally ? decoration.MaxHealth : (float?)null,
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

        private static TileDto MapTile(Engine.ForExternalUse.Synchronization.ObjectInterfaces.ITile tile, bool ally)
        {
            return new TileDto()
            {
                Height = tile.Height,
                NativeId = ally || tile.Revealed ? tile.NativeId : "grass",
                OwnerId = tile.OwnerId,
                TempActorId = tile.TempActorId,
                X = tile.X,
                Y = tile.Y,
                Unbearable = tile.Unbearable
            };
        }

        private static SynchronizerDto MapSynchronizer(ISynchronizer oldSynchronizer, string userId)
        {
            var userPlayerIds = oldSynchronizer.Players.Where(x => x.UserId == userId).Select(x => x.Id);
            var userTeams = oldSynchronizer.Players.Where(x => x.UserId == userId && x.Team != null).Select(x => x.Team).Distinct().ToList();
            var tileSet = oldSynchronizer.TileSet;
            return new SynchronizerDto()
            {
                ChangedActors = oldSynchronizer.ChangedActors.Select(x => MapActor(x, userTeams.Contains(x.Team) || userPlayerIds.Contains(x.OwnerId))),
                DeletedActors = oldSynchronizer.DeletedActors,
                ChangedDecorations = oldSynchronizer.ChangedDecorations.Select(x => MapDecoration(x, userTeams.Contains(x.Team) || userPlayerIds.Contains(x.OwnerId))),
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
                ChangedTiles = oldSynchronizer.ChangedTiles.Select(x => MapTile(x, userTeams.Contains(x.Team) || userPlayerIds.Contains(x.OwnerId))),
                TempActor = oldSynchronizer.TempActor,
                TempDecoration = oldSynchronizer.TempDecoration,
                TilesetHeight = tileSet.GetLength(1),
                TilesetWidth = tileSet.GetLength(0)
            };
        }

        public static bool CalculateReward(ref SynchronizerDto synchronizer, IScene scene, string playerId)
        {
            var currentPlayer = scene.ShortPlayers.First(x => x.Id == playerId);
            var update = currentPlayer.TryRedeemPlayerStatusHash(out int? statusHash);
            if (statusHash.HasValue)
            {
                var rewardRandom = new Random(statusHash.Value);
                switch (currentPlayer.Status)
                {
                    case Engine.Helpers.PlayerStatus.Victorious:
                        synchronizer.Reward = new RewardDto()
                        {
                            Experience = VictoryReward
                        };

                        break;
                    case Engine.Helpers.PlayerStatus.Defeated:
                        synchronizer.Reward = new RewardDto()
                        {
                            Experience = DefeatReward
                        };

                        break;
                    case Engine.Helpers.PlayerStatus.Left:
                        return false;
                }
            }

            return update;
        }

        public static SynchronizerDto MapSynchronizer(ISyncEventArgs syncEventArgs, string userId)
        {
            var synchronizer = MapSynchronizer(syncEventArgs.SyncInfo, userId);
            synchronizer.Id = syncEventArgs.Scene.Id;
            synchronizer.RoundsPassed = syncEventArgs.Scene.PassedTime;
            synchronizer.TargetX = syncEventArgs.TargetX;
            synchronizer.TargetY = syncEventArgs.TargetY;
            synchronizer.SkillActionId = syncEventArgs.SkillActionId;
            synchronizer.ActorId = syncEventArgs.ActorId;
            synchronizer.Version = syncEventArgs.Version;
            synchronizer.TurnTime = syncEventArgs.Scene.RemainedTurnTime;
            return synchronizer;
        }

        public static SynchronizerDto MapSynchronizer(ISynchronizer oldSynchronizer, IScene scene, string userId)
        {
            var synchronizer = MapSynchronizer(oldSynchronizer, userId);
            synchronizer.Id = scene.Id;
            synchronizer.RoundsPassed = scene.PassedTime;
            synchronizer.Version = scene.Version;
            synchronizer.TurnTime = scene.RemainedTurnTime;
            return synchronizer;
        }

        public static SynchronizerDto GetFullSynchronizationData(IScene scene, string userId)
        {
            var synchronizer = scene.GetFullSynchronizationData();
            return MapSynchronizer(synchronizer, scene, userId);
        }
    }
}