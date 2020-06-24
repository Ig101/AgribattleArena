namespace ProjectArena.Infrastructure.Models.Battle.Synchronization
{
    public class SkillDto
    {
        public int Id { get; set; }

        public int Range { get; set; }

        public string NativeId { get; set; }

        public string Visualization { get; set; }

        public float Cd { get; set; }

        public int Cost { get; set; }

        public float PreparationTime { get; set; }

        public TargetsDto AvailableTargets { get; set; }

        public bool OnlyVisibleTargets { get; set; }
    }
}
