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
using ProjectArena.Engine.ForExternalUse;
using ProjectArena.Engine.ForExternalUse.Generation.ObjectInterfaces;
using ProjectArena.Engine.ForExternalUse.Synchronization;
using ProjectArena.Infrastructure.Models.Battle.Synchronization;

namespace ProjectArena.Domain.BattleService
{
    public class BattleService : IBattleService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly INativeManager _nativeManager;
        private readonly IList<IScene> _scenes;
        private readonly Random _random;
        private long _sceneEnumerator;

        public BattleService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _nativeManager = SetupNativeManager();
            _scenes = new List<IScene>();
            _sceneEnumerator = 0;
            _random = new Random();
        }

        private INativeManager SetupNativeManager()
        {
            return null;
        }

        public async Task StartNewBattleAsync(SceneMode mode, IEnumerable<UserInQueue> users)
        {
            var battleHub = _serviceProvider.GetRequiredService<IHubContext<ArenaHub.ArenaHub>>();

            var userIds = users.Select(x => x.UserId).ToList();
            await battleHub.Clients.Users(userIds).SendAsync("BattlePrepare");
            var tempSceneId = _sceneEnumerator;
            _sceneEnumerator++;
            if (_sceneEnumerator == long.MaxValue)
            {
                _sceneEnumerator = 0;
            }

            var players = new List<IPlayer>(users.Count());

            // TODO DefaultScene
            /*
            foreach (string id in userIds)
            {
                players.Add(EngineHelper.CreatePlayerForGeneration(id, null, profile.Actors.Select(
                    x => EngineHelper.CreateActorForGeneration(x.Id, x.ActorNative, x.AttackingSkillNative, x.Strength, x.Willpower, x.Constitution, x.Speed,
                    x.Skills.Select(k => k.Native), x.ActionPointsIncome, null))));
            }*/
            _scenes.Add(Engine.ForExternalUse.EngineHelper.EngineHelper.CreateNewScene(tempSceneId, players, mode.Generator, _nativeManager, mode.VarManager, _random.Next(), SynchronizationInfoEventHandler));
        }

        private void SynchronizationInfoEventHandler(object sender, ISyncEventArgs e)
        {
            var battleHub = _serviceProvider.GetRequiredService<IHubContext<ArenaHub.ArenaHub>>();

            string actionName = BattleHelper.GetBattleActionMethodName(e.Action);
            var synchronizer = BattleHelper.MapSynchronizer(e);
            battleHub.Clients.Users(e.Scene.ShortPlayers.Select(x => x.Id).ToList())?.SendAsync(actionName, synchronizer);
            if (e.Action == Engine.Helpers.Action.EndGame)
            {
                _scenes.Remove(e.Scene);

                // TODO Rewards
            }
        }

        public void EngineTimeProcessing(double seconds)
        {
            foreach (var scene in _scenes)
            {
                scene.UpdateTime((int)(seconds * 1000));
            }
        }

        public SynchronizerDto IsUserInBattle(string userId)
        {
            foreach (var scene in _scenes)
            {
                if (scene.ShortPlayers.FirstOrDefault(x => x.Id == userId) != null)
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
                if (scene.ShortPlayers.FirstOrDefault(x => x.Id == userId) != null)
                {
                    return scene;
                }
            }

            return null;
        }
    }
}