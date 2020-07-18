using System;
using ProjectArena.Content.SkillNatives.Base;
using ProjectArena.Engine.ForExternalUse;
using ProjectArena.Engine.Helpers;

namespace ProjectArena.Content.SkillNatives
{
    public class Charge : INative
    {
        public void Fill(INativeManager nativeManager)
        {
            nativeManager.AddSkillNative(
                "charge",
                "charge",
                "charge",
                new[] { "damage", "target", "movement", "control", "weapon", "direct", "physic" },
                7,
                4,
                3,
                10,
                new Targets()
                {
                    Allies = true,
                    NotAllies = true,
                    Bearable = true,
                    Decorations = true
                },
                true,
                (scene, owner, targetTile, skill) =>
                {
                    targetTile.DoDamageAttack(skill);
                    if (Misc.RangeBetween(owner.X, owner.Y, targetTile.X, targetTile.Y) >= 2)
                    {
                        targetTile.AddBuff("stun", 0, 1, true);
                    }

                    var angle = Misc.AngleBetween(targetTile.X, targetTile.Y, owner.X, owner.Y);
                    var sin = (float)Math.Sin(angle);
                    var cos = (float)Math.Cos(angle);
                    var x = (float)targetTile.X;
                    var y = (float)targetTile.Y;
                    if (targetTile.TempObject != null)
                    {
                        while ((int)Math.Round(x, MidpointRounding.AwayFromZero) == targetTile.X && (int)Math.Round(y, MidpointRounding.AwayFromZero) == targetTile.Y)
                        {
                            x += cos;
                            y += sin;
                        }
                    }

                    owner.ChangePosition(scene.Tiles[(int)Math.Round(x, MidpointRounding.AwayFromZero)][(int)Math.Round(y, MidpointRounding.AwayFromZero)], true);
                });
        }
    }
}