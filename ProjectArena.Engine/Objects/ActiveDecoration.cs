using ProjectArena.Engine.Helpers;
using ProjectArena.Engine.Natives;
using ProjectArena.Engine.Objects.Abstract;
using ProjectArena.Engine.Objects.Immaterial;

namespace ProjectArena.Engine.Objects
{
    public class ActiveDecoration : TileObject
    {
        public new ActiveDecorationNative Native => (ActiveDecorationNative)base.Native;

        public float Mod { get; set; }

        public ActiveDecoration(ISceneParentRef parent, IPlayerParentRef owner, ITileParentRef tempTile, float? z, int? maxHealth, TagSynergy[] armor, ActiveDecorationNative native, float? mod)
            : base(parent, owner, tempTile, z ?? native.DefaultZ, new DamageModel(maxHealth ?? native.DefaultHealth, armor ?? native.DefaultArmor), native)
        {
            this.Mod = mod ?? native.DefaultMod;
            this.InitiativePosition += 1;
        }

        public override void Update(float time)
        {
            this.InitiativePosition -= time;
            if (time > 0)
            {
                this.Affected = true;
            }
        }

        public void Cast()
        {
            Native.Action?.Invoke(Parent, this);
        }

        public override void EndTurn()
        {
            this.InitiativePosition += 1;
        }

        public override bool StartTurn()
        {
            return true;
        }

        public override void OnDeathAction()
        {
            Native?.OnDeathAction(Parent, this);
        }
    }
}
