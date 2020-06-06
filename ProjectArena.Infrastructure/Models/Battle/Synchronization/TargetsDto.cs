namespace ProjectArena.Infrastructure.Models.Battle.Synchronization
{
    public class TargetsDto
    {
        public bool Self { get; set; }

        public bool Allies { get; set; }

        public bool NotAllies { get; set; }

        public bool Bearable { get; set; }

        public bool Unbearable { get; set; }

        public bool Decorations { get; set; }
    }
}