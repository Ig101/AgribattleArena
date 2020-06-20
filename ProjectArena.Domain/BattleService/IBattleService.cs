using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectArena.Domain.BattleService.Models;
using ProjectArena.Domain.QueueService.Models;
using ProjectArena.Engine.ForExternalUse;
using ProjectArena.Infrastructure.Models.Battle.Synchronization;

namespace ProjectArena.Domain.BattleService
{
    public interface IBattleService
    {
        Task StartNewBattleAsync(SceneMode mode, IEnumerable<UserInQueue> users);

        void EngineTimeProcessing(double seconds);

        IScene GetUserScene(string userId, Guid sceneId);

        bool IsUserInBattle(string userId);

        SynchronizerDto GetUserSynchronizationInfo(string userId, Guid? sceneId);

        IEnumerable<SynchronizerDto> GetAllUserSynchronizationInfos(string userId);

        void Init();

        bool LeaveScene(string userId, Guid sceneId);
    }
}