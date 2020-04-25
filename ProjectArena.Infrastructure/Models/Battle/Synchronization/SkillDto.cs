namespace ProjectArena.Infrastructure.Models.Battle.Synchronization
{
    public class SkillDto
    {
        public int Id { get; set; }

        public int Range { get; set; }

        public string NativeId { get; set; }

        public float Cd { get; set; }

        public float Mod { get; set; }

        public int Cost { get; set; }

        public float PreparationTime { get; set; }
    }
}
