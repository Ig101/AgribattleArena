using ProjectArena.Infrastructure.Enums;

namespace ProjectArena.Infrastructure.Models.Battle
{
    public class PlayerSynchronizationDto
    {
        public string Id { get; set; }

        public PlayerStatus BattlePlayerStatus { get; set; }
    }
}