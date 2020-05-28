using System;
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
                target.BuffManager.AddBuff("stun", 0, 1 / owner.Initiative);
            }
        }

        public static void DoChargeStun(ISceneParentRef scene, IActorParentRef owner, Tile targetTile, Skill skill)
        {
            if (Misc.RangeBetween(owner.X, owner.Y, targetTile.X, targetTile.Y) >= 2)
            {
                DoOneTurnStun(scene, owner, targetTile, skill);
            }
        }

        public static void DoMove(ISceneParentRef scene, IActorParentRef owner, Tile targetTile, Skill skill)
        {
            owner.ChangePosition(targetTile, true);
        }

        public static void DoChargeMove(ISceneParentRef scene, IActorParentRef owner, Tile targetTile, Skill skill)
        {
            var angle = Misc.AngleBetween(targetTile.X, targetTile.Y, owner.X, owner.Y);
            var sin = (float)(Math.Sin(angle) * 0.5);
            var cos = (float)(Math.Cos(angle) * 0.5);
            var x = (float)targetTile.X;
            var y = (float)targetTile.Y;
            while ((int)x == targetTile.X && (int)y == targetTile.Y)
            {
                x += cos;
                y += sin;
            }

            owner.ChangePosition(scene.Tiles[(int)x][(int)y], true);
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

        public static void MakePowerPlace(ISceneParentRef scene, IActorParentRef owner, Tile targetTile, Skill skill)
        {
            scene.ChangeTile("powerplace", targetTile.X, targetTile.Y, targetTile.Height, owner.Owner);
        }
    }
}
