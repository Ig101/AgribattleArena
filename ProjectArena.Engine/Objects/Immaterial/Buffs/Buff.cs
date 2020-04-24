using ProjectArena.Engine.Natives;
using ProjectArena.Engine.Objects.Abstract;

namespace ProjectArena.Engine.Objects.Immaterial.Buffs
{
    public class Buff : IdObject
    {
        public IBuffManagerParentRef Manager { get; }

        public BuffNative Native { get; }

        public float Mod { get; set; }

        public float? Duration { get; set; }

        public Buff(IBuffManagerParentRef manager, BuffNative native, float? mod, float? duration)
            : base(manager.Parent.Parent)
        {
            this.Mod = mod ?? native.DefaultMod;
            this.Duration = duration ?? native.DefaultDuration;
            this.Native = native;
            this.Manager = manager;
        }

        public void Update(float time)
        {
            if (Duration != null)
            {
                Duration -= time;
            }

            Native.Action?.Invoke(Parent, Manager.Parent, this, time);
        }

        public void Purge()
        {
            Native.OnPurgeAction?.Invoke(Parent, Manager.Parent, this);
        }
    }
}
