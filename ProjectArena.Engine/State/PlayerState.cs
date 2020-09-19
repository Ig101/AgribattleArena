using System.Collections.Generic;
using ProjectArena.Infrastructure.Enums;
using ProjectArena.Infrastructure.Models.Battle;

namespace ProjectArena.Engine.State
{
    public class PlayerState
    {
        public string Id { get; set; }

        public RewardInfoDto Reward { get; set; }

        public IEnumerable<int> KeyActorIds { get; set; }

        public PlayerStatus BattlePlayerStatus { get; set; }
    }
}