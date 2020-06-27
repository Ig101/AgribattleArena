using ProjectArena.Engine.Helpers;
using ProjectArena.Engine.Natives;
using ProjectArena.Engine.Objects.Abstract;
using ProjectArena.Engine.Objects.Immaterial;

namespace ProjectArena.Engine.Objects
{
    public class ActiveDecoration : TileObject
    {
        public new ActiveDecorationNative Native => (ActiveDecorationNative)base.Native;

        public string Visualization { get; set; }

        public float Mod { get; set; }

        public ActiveDecoration(Scene parent, Player owner, Tile tempTile, string visualization, float? z, int? maxHealth, TagSynergy[] armor, ActiveDecorationNative native, float? mod)
            : base(parent, owner, tempTile, z ?? native.DefaultZ, new DamageModel(maxHealth ?? native.DefaultHealth, armor ?? native.DefaultArmor), native)
        {
            this.Visualization = visualization ?? native.DefaultVisualisation;
            this.Mod = mod ?? native.DefaultMod;
        }

        public override void OnHitAction(float amount)
        {
            Native.OnHitAction?.Invoke(Parent, this, amount);
        }

        public override void OnDeathAction()
        {
            TempTile.Native.OnDeathAction?.Invoke(Parent, TempTile, this);
            Native.OnDeathAction?.Invoke(Parent, this);
        }
    }
}
