using ProjectArena.Infrastructure.Enums;

namespace ProjectArena.Engine.State
{
    public class PlayerState
    {
        public string Id { get; set; }

        public PlayerStatus BattlePlayerStatus { get; set; }
    }
}