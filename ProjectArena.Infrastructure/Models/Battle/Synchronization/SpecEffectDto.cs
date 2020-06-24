namespace ProjectArena.Infrastructure.Models.Battle.Synchronization
{
    public class SpecEffectDto
    {
        public int Id { get; set; }

        public string OwnerId { get; set; }

        public bool IsAlive { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public float Z { get; set; }

        public float? Duration { get; set; }

        public string NativeId { get; set; }

        public string Visualization { get; set; }
    }
}
