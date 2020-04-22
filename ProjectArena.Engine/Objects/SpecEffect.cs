using System;
using AgribattleArena.Engine.Natives;
using AgribattleArena.Engine.Objects.Abstract;

namespace AgribattleArena.Engine.Objects
{
    public class SpecEffect : GameObject
    {
        public bool Affected { get; set; }

        public float? Duration { get; set; }

        public float Mod { get; set; }

        public SpecEffectNative Native { get; set; }

        public SpecEffect(ISceneParentRef parent, IPlayerParentRef owner, int x, int y, float? z, SpecEffectNative native, float? duration, float? mod)
            : base(parent, owner, x, y, z ?? native.DefaultZ)
        {
            this.Duration = duration ?? native.DefaultDuration ?? null;
            this.Native = native;
            this.Mod = mod ?? native.DefaultMod;
            this.Affected = true;
        }

        public override void Update(float time)
        {
            this.Native.Action?.Invoke(Parent, this, Duration != null ? Math.Min(Duration.Value, time) : time);
            this.Duration -= time;
            if (this.Duration <= 0)
            {
                IsAlive = false;
            }
        }

        public override void OnDeathAction()
        {
            Native.OnDeathAction?.Invoke(Parent, this);
        }
    }
}
