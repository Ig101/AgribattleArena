using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using ProjectArena.Domain.BattleService.Helpers;
using ProjectArena.Domain.BattleService.Models;
using ProjectArena.Domain.QueueService.Models;
using ProjectArena.Domain.Registry;
using ProjectArena.Engine.ForExternalUse;
using ProjectArena.Engine.ForExternalUse.EngineHelper;
using ProjectArena.Engine.ForExternalUse.Generation.ObjectInterfaces;
using ProjectArena.Engine.ForExternalUse.Synchronization;
using ProjectArena.Infrastructure.Models.Battle.Synchronization;

namespace ProjectArena.Domain.BattleService
{
    public class BattleService : IBattleService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IList<IScene> _scenes;
        private readonly Random _random;
        private INativeManager _nativeManager;
        private long _sceneEnumerator;

        public BattleService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _scenes = new List<IScene>();
            _sceneEnumerator = 0;
            _random = new Random();
        }

        private async Task SetupNewNativeManagerAsync()
        {
            var nativeManager = EngineHelper.CreateNativeManager();
            var registryContext = _serviceProvider.GetRequiredService<RegistryContext>();
            var actors = await registryContext.ActorNatives.GetAsync(x => true);
            foreach (var actor in actors)
            {
                nativeManager.AddActorNative(
                    actor.Id,
                    actor.Tags.ToArray(),
                    actor.DefaultZ,
                    actor.Armor.Select(x => new ProjectArena.Engine.Helpers.TagSynergy(x.SelfTag, x.TargetTag, x.Mod)).ToArray());
            }

            var decorations = await registryContext.DecorationNatives.GetAsync(x => true);
            foreach (var decoration in decorations)
            {
                nativeManager.AddDecorationNative(
                    decoration.Id,
                    decoration.Tags.ToArray(),
                    decoration.DefaultArmor.Select(x => new ProjectArena.Engine.Helpers.TagSynergy(x.SelfTag, x.TargetTag, x.Mod)).ToArray(),
                    decoration.DefaultHealth,
                    decoration.DefaultZ,
                    decoration.DefaultMod,
                    decoration.Actions,
                    decoration.OnDeathActions);
            }

            var effects = await registryContext.EffectNatives.GetAsync(x => true);
            foreach (var effect in effects)
            {
                nativeManager.AddEffectNative(
                    effect.Id,
                    effect.Tags.ToArray(),
                    effect.DefaultZ,
                    effect.DefaultDuration,
                    effect.DefaultMod,
                    effect.Actions,
                    effect.OnDeathActions);
            }

            var buffs = await registryContext.BuffNatives.GetAsync(x => true);
            foreach (var buff in buffs)
            {
                nativeManager.AddBuffNative(
                    buff.Id,
                    buff.Tags.ToArray(),
                    buff.Eternal,
                    buff.Repeatable,
                    buff.SummarizeLength,
                    buff.DefaultDuration,
                    buff.DefaultMod,
                    buff.Actions,
                    buff.Appliers,
                    buff.OnPurgeActions);
            }

            var roleModels = await registryContext.RoleModelNatives.GetAsync(x => true);
            foreach (var roleModel in roleModels)
            {
                nativeManager.AddRoleModelNative(
                    roleModel.Id,
                    roleModel.DefaultStrength,
                    roleModel.DefaultWillpower,
                    roleModel.DefaultConstitution,
                    roleModel.DefaultSpeed,
                    roleModel.DefaultActionPointsIncome,
                    roleModel.AttackingSkill,
                    roleModel.Skills.ToArray());
            }

            var skills = await registryContext.SkillNatives.GetAsync(x => true);
            foreach (var skill in skills)
            {
                nativeManager.AddSkillNative(
                    skill.Id,
                    skill.Tags.ToArray(),
                    skill.DefaultRange,
                    skill.DefaultCost,
                    skill.DefaultCd,
                    skill.DefaultMod,
                    skill.MeleeOnly,
                    skill.Actions);
            }

            var tiles = await registryContext.TileNatives.GetAsync(x => true);
            foreach (var tile in tiles)
            {
                nativeManager.AddTileNative(
                    tile.Id,
                    tile.Tags.ToArray(),
                    tile.Flat,
                    tile.DefaultHeight,
                    tile.Unbearable,
                    tile.DefaultMod,
                    tile.Actions,
                    tile.OnStepActions);
            }

            _nativeManager = nativeManager;
        }

        public void Init()
        {
            SetupNewNativeManagerAsync().Wait();
        }

        public void StartNewBattle(SceneMode mode, IEnumerable<UserInQueue> users)
        {
            var battleHub = _serviceProvider.GetRequiredService<IHubContext<ArenaHub.ArenaHub>>();

            var userIds = users.Select(x => x.UserId).ToList();
            var tempSceneId = _sceneEnumerator;
            _sceneEnumerator++;
            if (_sceneEnumerator == long.MaxValue)
            {
                _sceneEnumerator = 0;
            }

            var players = new List<IPlayer>(users.Count());

            foreach (string id in userIds)
            {
                var playerActors = new List<IActor>(5);
                for (int i = 0; i < 10; i++)
                {
                    playerActors.Add(EngineHelper.CreateActorForGeneration(Guid.NewGuid(), "adventurer", "slash", 10, 10, 10, 10, new[] { "explosion" }, 6, null));
                }

                players.Add(EngineHelper.CreatePlayerForGeneration(id, null, playerActors));
            }

            var scene = EngineHelper.CreateNewScene(tempSceneId, players, mode.Generator, _nativeManager, mode.VarManager, _random.Next(), SynchronizationInfoEventHandler);
            _scenes.Add(scene);
        }

        private void SynchronizationInfoEventHandler(object sender, ISyncEventArgs e)
        {
            var battleHub = _serviceProvider.GetRequiredService<IHubContext<ArenaHub.ArenaHub>>();

            string actionName = BattleHelper.GetBattleActionMethodName(e.Action);
            var synchronizer = BattleHelper.MapSynchronizer(e);
            battleHub.Clients.Users(e.Scene.ShortPlayers.Select(x => x.Id).ToList())?.SendAsync(actionName, synchronizer);
            if (e.Action == Engine.Helpers.Action.EndGame)
            {
                // TODO Rewards
            }
        }

        public void EngineTimeProcessing(double seconds)
        {
            for (int i = 0; i < _scenes.Count; i++)
            {
                _scenes[i].UpdateTime((float)seconds);
                if (!_scenes[i].IsActive)
                {
                    _scenes.RemoveAt(i);
                    i--;
                }
            }
        }

        public bool IsUserInBattle(string userId)
        {
            foreach (var scene in _scenes)
            {
                if (scene.IsActive && scene.ShortPlayers.FirstOrDefault(x => x.Id == userId) != null)
                {
                    return true;
                }
            }

            return false;
        }

        public SynchronizerDto GetUserSynchronizationInfo(string userId)
        {
            foreach (var scene in _scenes)
            {
                if (scene.IsActive && scene.ShortPlayers.FirstOrDefault(x => x.Id == userId) != null)
                {
                    return BattleHelper.GetFullSynchronizationData(scene);
                }
            }

            return null;
        }

        public IScene GetUserScene(string userId)
        {
            foreach (var scene in _scenes)
            {
                if (scene.IsActive && scene.ShortPlayers.FirstOrDefault(x => x.Id == userId) != null)
                {
                    return scene;
                }
            }

            return null;
        }
    }
}