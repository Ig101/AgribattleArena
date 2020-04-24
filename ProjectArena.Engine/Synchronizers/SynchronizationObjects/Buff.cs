using ProjectArena.Engine.ForExternalUse.Synchronization.ObjectInterfaces;

namespace ProjectArena.Engine.Synchronizers.SynchronizationObjects
{
    internal class Buff : IBuff
    {
        public int Id { get; }

        public string NativeId { get; }

        public float Mod { get; }

        public float? Duration { get; }

        public Buff(Objects.Immaterial.Buffs.Buff buff)
        {
            this.Id = buff.Id;
            this.NativeId = buff.Native.Id;
            this.Mod = buff.Mod;
            this.Duration = buff.Duration;
        }
    }
}
