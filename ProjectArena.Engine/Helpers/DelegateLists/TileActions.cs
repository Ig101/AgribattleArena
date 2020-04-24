using System.Linq;
using ProjectArena.Engine.Objects;

namespace ProjectArena.Engine.Helpers.DelegateLists
{
    public class TileActions
    {
        public delegate void Action(ISceneParentRef parent, Tile tile, float time);

        public delegate void OnStepAction(ISceneParentRef parent, Tile tile);

        public static void DoDamageOnStep(ISceneParentRef parent, Tile tile)
        {
            DoDamage(parent, tile, 1);
        }

        public static void DoDamage(ISceneParentRef parent, Tile tile, float time)
        {
            if (time > 0)
            {
                if (tile.TempObject != null)
                {
                    tile.TempObject.Damage(tile.Native.DefaultMod * time, tile.Native.Tags);
                }
            }
        }

        public static void PurgeTileEffects(ISceneParentRef parent, Tile tile)
        {
            if (tile.TempObject != null && tile.TempObject is Actor actor)
            {
                actor.BuffManager.RemoveBuffsByTagsCondition(x =>
                {
                    return x.Contains("tile");
                });
            }
        }

        public static void AddSpellDamageTile(ISceneParentRef parent, Tile tile)
        {
            if (tile.TempObject != null && tile.TempObject is Actor actor)
            {
                actor.BuffManager.AddBuff("tile_increase_spell_damage", tile.Native.DefaultMod, null);
            }
        }

        public static void ReducePureResistanceTile(ISceneParentRef parent, Tile tile)
        {
            if (tile.TempObject != null && tile.TempObject is Actor actor)
            {
                actor.BuffManager.AddBuff("tile_reduce_pure_resistance", tile.Native.DefaultMod, null);
            }
        }
    }
}
