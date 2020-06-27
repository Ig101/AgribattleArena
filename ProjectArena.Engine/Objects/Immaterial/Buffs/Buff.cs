using ProjectArena.Engine.Natives;
using ProjectArena.Engine.Objects.Abstract;

namespace ProjectArena.Engine.Objects.Immaterial.Buffs
{
    public class Buff : IdObject
    {
        public BuffManager Manager { get; }

        public BuffNative Native { get; }

        public float Mod { get; set; }

        public float? Duration { get; set; }

        public Buff(BuffManager manager, BuffNative native, float? mod, float? duration)
            : base(manager.Parent.Parent)
        {
            this.Mod = mod ?? native.DefaultMod;
            this.Duration = duration ?? native.DefaultDuration;
            this.Native = native;
            this.Manager = manager;
        }

        public void Update()
        {
            if (Duration != null)
            {
                Duration -= 1;
            }
        }

        public void Purge()
        {
            Native.OnPurgeAction?.Invoke(Parent, Manager.Parent, this);
        }
    }
}
