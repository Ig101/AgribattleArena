using ProjectArena.Content.SkillNatives.Base;
using ProjectArena.Engine.ForExternalUse;
using ProjectArena.Engine.Helpers;

namespace ProjectArena.Content.SkillNatives
{
    public class Warden : INative
    {
        public void Fill(INativeManager nativeManager)
        {
            nativeManager.AddSkillNative(
                "warden",
                "warden",
                "warden",
                new[] { "damage", "target", "physic", "ranged", "direct" },
                6,
                3,
                4,
                45,
                new Targets()
                {
                    Allies = true,
                    NotAllies = true,
                    Unbearable = true,
                    Bearable = true,
                    Decorations = true
                },
                true,
                (scene, owner, targetTile, skill) =>
                {
                    targetTile.DoDamageAttack(skill);
                });
        }
    }
}