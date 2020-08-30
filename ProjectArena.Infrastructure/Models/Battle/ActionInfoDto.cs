using ProjectArena.Infrastructure.Enums;

namespace ProjectArena.Infrastructure.Models.Battle
{
    public class ActionInfoDto
    {
        public ActorReferenceDto Actor { get; set; }

        public string Id { get; set; }

        public ActionType Type { get; set; }

        public int? X { get; set; }

        public int? Y { get; set; }

        public int? TargetId { get; set; }
    }
}