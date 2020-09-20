using ProjectArena.Infrastructure.Enums;

namespace ProjectArena.Infrastructure.Models.Battle
{
    public class SynchronizationMessageDto
    {
        public SynchronizationMessageType Id { get; set; }

        public string SceneId { get; set; }

        public int Version { get; set; }

        public string Code { get; set; }

        public ActionInfoDto Action { get; set; }

        public RewardInfoDto Reward { get; set; }

        public StartTurnInfoDto StartTurnInfo { get; set; }
    }
}