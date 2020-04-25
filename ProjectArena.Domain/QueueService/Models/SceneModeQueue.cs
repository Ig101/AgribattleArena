using System.Collections;
using System.Collections.Generic;
using ProjectArena.Domain.BattleService.Models;

namespace ProjectArena.Domain.QueueService.Models
{
    public class SceneModeQueue
    {
        public IList<UserInQueue> Queue { get; set; }

        public SceneMode Mode { get; set; }
    }
}