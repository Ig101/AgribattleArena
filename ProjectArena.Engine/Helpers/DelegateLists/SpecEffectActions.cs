using ProjectArena.Engine.Objects;
using ProjectArena.Engine.Objects.Abstract;

namespace ProjectArena.Engine.Helpers.DelegateLists
{
    public class SpecEffectActions
    {
        public delegate void Action(ISceneParentRef parent, SpecEffect effect, float time);

        public delegate void OnDeathAction(ISceneParentRef parent, SpecEffect effect);

        public static void DoDamageTempTile(ISceneParentRef parent, SpecEffect effect, float time)
        {
            if (time > 0)
            {
                TileObject target = parent.Tiles[effect.X][effect.Y].TempObject;
                if (target != null)
                {
                    target.Damage(effect.Mod * time, effect.Native.Tags);
                }
            }
        }

        public static void DoDamageTempTileDeath(ISceneParentRef parent, SpecEffect effect)
        {
            DoDamageTempTile(parent, effect, 1);
        }
    }
}
