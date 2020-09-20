using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectArena.Domain.BattleService.Models;
using ProjectArena.Domain.QueueService.Models;
using ProjectArena.Engine;
using ProjectArena.Infrastructure.Models.Battle;

namespace ProjectArena.Domain.BattleService
{
    public interface IBattleService
    {
        Task StartNewBattleAsync(SceneMode mode, IEnumerable<UserInQueue> users);

        void EngineTimeProcessing(double seconds);

        Scene GetUserScene(string userId, Guid sceneId);

        bool IsUserInBattle(string userId);

        FullSynchronizationInfoDto GetUserSynchronizationInfo(string userId, Guid? sceneId);

        IEnumerable<FullSynchronizationInfoDto> GetAllUserSynchronizationInfos(string userId);

        void Init();

        bool LeaveScene(string userId, Guid sceneId);
    }
}