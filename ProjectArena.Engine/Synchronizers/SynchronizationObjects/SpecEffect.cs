using ProjectArena.Engine.ForExternalUse.Synchronization.ObjectInterfaces;

namespace ProjectArena.Engine.Synchronizers.SynchronizationObjects
{
    internal class SpecEffect : ISpecEffect
    {
        public int Id { get; }

        public string Visualization { get; }

        public string OwnerId { get; }

        public int? Team { get; }

        public bool IsAlive { get; }

        public int X { get; }

        public int Y { get; }

        public float Z { get; }

        public float? Duration { get; }

        public float Mod { get; }

        public string NativeId { get; }

        public SpecEffect(Objects.SpecEffect specEffect)
        {
            this.Id = specEffect.Id;
            this.Visualization = specEffect.Visualization;
            this.OwnerId = specEffect.Owner?.Id;
            this.Team = specEffect.Owner?.Team;
            this.IsAlive = specEffect.IsAlive;
            this.X = specEffect.X;
            this.Y = specEffect.Y;
            this.Z = specEffect.Z;
            this.Duration = specEffect.Duration;
            this.Mod = specEffect.Mod;
            this.NativeId = specEffect.Native.Id;
        }
    }
}
