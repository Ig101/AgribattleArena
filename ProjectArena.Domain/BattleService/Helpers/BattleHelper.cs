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
                            VarManager = EngineHelper.CreateVarManager(4, 0.05f, 0.05f, 0.03f, 0.4f),
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
                            VarManager = EngineHelper.CreateVarManager(4, 0.05f, 0.05f, 0.03f, 0.4f),
                            MaxPlayers = 2,
                            TimeTillBot = 6,
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
                AttackingSkill = ally || actor.AttackingSkill.Revealed ? new SkillDto()
                {
                    Cd = actor.AttackingSkill.Cd,
                    Id = actor.AttackingSkill.Id,
                    NativeId = actor.AttackingSkill.NativeId,
                    Visualization = ally ? actor.AttackingSkill.Visualization : actor.AttackingSkill.EnemyVisualization,
                    PreparationTime = actor.AttackingSkill.PreparationTime,
                    Range = actor.AttackingSkill.Range,
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
                Visualization = ally ? actor.Visualization : actor.EnemyVisualization,
                Id = actor.Id,
                ExternalId = actor.ExternalId,
                Initiative = actor.Initiative,
                MaxHealth = actor.HealthRevealed || ally ? actor.MaxHealth : (int?)null,
                Health = actor.HealthRevealed || ally ? actor.Health : (float?)null,
                OwnerId = actor.OwnerId,
                Skills = actor.Skills.Where(k => ally || k.Revealed).Select(k => new SkillDto()
                {
                    Cd = k.Cd,
                    Id = k.Id,
                    NativeId = k.NativeId,
                    Visualization = ally ? k.Visualization : k.EnemyVisualization,
                    PreparationTime = k.PreparationTime,
                    Range = k.Range,
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
                Health = decoration.HealthRevealed || ally ? decoration.Health : (float?)null,
                Id = decoration.Id,
                IsAlive = decoration.IsAlive,
                MaxHealth = decoration.HealthRevealed || ally ? decoration.MaxHealth : (float?)null,
                Visualization = decoration.Visualization,
                NativeId = decoration.NativeId,
                OwnerId = decoration.OwnerId,
                X = decoration.X,
                Y = decoration.Y,
                Z = decoration.Z
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
                Players = oldSynchronizer.Players.Select(x => new PlayerDto()
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    KeyActorsSync = x.KeyActorsSync,
                    Status = (PlayerStatus)(int)x.Status,
                    Team = x.Team,
                    PlayerActorId = x.PlayerActorId
                }),
                ChangedTiles = oldSynchronizer.ChangedTiles.Select(x => MapTile(x)),
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
            return synchronizer;
        }

        public static SynchronizerDto MapSynchronizer(ISynchronizer oldSynchronizer, IScene scene, string userId)
        {
            var synchronizer = MapSynchronizer(oldSynchronizer, userId);
            synchronizer.Id = scene.Id;
            synchronizer.RoundsPassed = scene.PassedTime;
            synchronizer.Version = scene.Version;
            return synchronizer;
        }

        public static SynchronizerDto GetFullSynchronizationData(IScene scene, string userId)
        {
            var synchronizer = scene.GetFullSynchronizationData();
            return MapSynchronizer(synchronizer, scene, userId);
        }
    }
}