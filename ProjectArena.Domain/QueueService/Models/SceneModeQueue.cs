using System.Collections.Concurrent;
using System.Collections.Generic;
using ProjectArena.Domain.BattleService.Models;

namespace ProjectArena.Domain.QueueService.Models
{
    public class SceneModeQueue
    {
        public HashSet<UserInQueue> Queue { get; set; }

        public SceneMode Mode { get; set; }
    }
}