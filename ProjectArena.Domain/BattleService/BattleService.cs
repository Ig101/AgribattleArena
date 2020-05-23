using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using ProjectArena.Domain.BattleService.Helpers;
using ProjectArena.Domain.BattleService.Helpers.NativeContainers;
using ProjectArena.Domain.BattleService.Models;
using ProjectArena.Domain.Game;
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

        private void SetupNewNativeManager()
        {
            var nativeManager = EngineHelper.CreateNativeManager();
            nativeManager.FillBuffNatives();
            nativeManager.FillActorNatives();
            nativeManager.FillSkillNatives();
            nativeManager.FillDecorationNatives();
            nativeManager.FillEffectNatives();
            nativeManager.FillRoleModelNatives();
            nativeManager.FillTileNatives();
            _nativeManager = nativeManager;
        }

        public void Init()
        {
            SetupNewNativeManager();
        }

        public async Task StartNewBattleAsync(SceneMode mode, IEnumerable<UserInQueue> users)
        {
            var battleHub = _serviceProvider.GetRequiredService<IHubContext<ArenaHub.ArenaHub>>();
            var gameContext = _serviceProvider.GetRequiredService<GameContext>();

            var userIds = users.Select(x => x.UserId).ToList();
            var tempSceneId = _sceneEnumerator;
            _sceneEnumerator++;
            if (_sceneEnumerator == long.MaxValue)
            {
                _sceneEnumerator = 0;
            }

            var players = new List<IPlayer>(users.Count());

            var characters = await gameContext.Characters.GetAsync(x => userIds.Contains(x.RosterUserId) && !x.Deleted);

            foreach (string id in userIds)
            {
                var playerActors = new List<IActor>();
                playerActors.AddRange(
                    characters
                    .Where(x => x.RosterUserId == id)
                    .Select(x =>
                    {
                        return EngineHelper.CreateActorForGeneration(
                            Guid.Parse(x.Id),
                            "adventurer",
                            "slash",
                            BattleHelper.DefaultStrength,
                            BattleHelper.DefaultWillpower,
                            BattleHelper.DefaultConstitution,
                            BattleHelper.DefaultSpeed,
                            new[] { "explosion" },
                            6,
                            null);
                    }));

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