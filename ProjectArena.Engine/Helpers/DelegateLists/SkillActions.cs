using ProjectArena.Engine.Objects;
using ProjectArena.Engine.Objects.Immaterial;

namespace ProjectArena.Engine.Helpers.DelegateLists
{
    public class SkillActions
    {
        public delegate void Action(ISceneParentRef scene, IActorParentRef owner, Tile targetTile, Skill skill);

        public static void DoDamageAttack(ISceneParentRef scene, IActorParentRef owner, Tile targetTile, Skill skill)
        {
            if (targetTile.TempObject != null)
            {
                float mod = skill.CalculateModAttackPower(targetTile.TempObject.Native.Tags);
                targetTile.TempObject.Damage(mod, skill.AggregatedTags);
            }
        }

        public static void DoDamageSkill(ISceneParentRef scene, IActorParentRef owner, Tile targetTile, Skill skill)
        {
            if (targetTile.TempObject != null)
            {
                float mod = skill.CalculateModSkillPower(targetTile.TempObject.Native.Tags);
                targetTile.TempObject.Damage(mod, skill.AggregatedTags);
            }
        }

        public static void DoOneTurnStun(ISceneParentRef scene, IActorParentRef owner, Tile targetTile, Skill skill)
        {
            if (targetTile.TempObject != null && targetTile.TempObject is Actor target)
            {
                target.BuffManager.AddBuff("stun", 0, (1 / owner.Initiative) + 0.05f);
            }
        }

        public static void DoSmallAoeDamageSkill(ISceneParentRef scene, IActorParentRef owner, Tile targetTile, Skill skill)
        {
            for (int x = targetTile.X - 1; x <= targetTile.X + 1; x++)
            {
                for (int y = targetTile.Y - 1; y <= targetTile.Y + 1; y++)
                {
                    if (x >= 0 && y >= 0 && x < scene.Tiles.Length && y < scene.Tiles[0].Length)
                    {
                        DoDamageSkill(scene, owner, scene.Tiles[x][y], skill);
                    }
                }
            }
        }

        public static void DoSmallAoeOneTurnStun(ISceneParentRef scene, IActorParentRef owner, Tile targetTile, Skill skill)
        {
            for (int x = targetTile.X - 1; x <= targetTile.X + 1; x++)
            {
                for (int y = targetTile.Y - 1; y <= targetTile.Y + 1; y++)
                {
                    if (x >= 0 && y >= 0 && x < scene.Tiles.Length && y < scene.Tiles[0].Length)
                    {
                        DoOneTurnStun(scene, owner, scene.Tiles[x][y], skill);
                    }
                }
            }
        }

        public static void MakePureLandOnSelf(ISceneParentRef scene, IActorParentRef owner, Tile targetTile, Skill skill)
        {
            scene.ChangeTile("tile_pure", targetTile.X, targetTile.Y, targetTile.Height, owner.Owner);
        }
    }
}
