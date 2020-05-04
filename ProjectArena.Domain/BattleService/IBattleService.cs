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
        void StartNewBattle(SceneMode mode, IEnumerable<UserInQueue> users);

        void EngineTimeProcessing(double seconds);

        IScene GetUserScene(string userId);

        bool IsUserInBattle(string userId);

        SynchronizerDto GetUserSynchronizationInfo(string userId);

        void Init();
    }
}