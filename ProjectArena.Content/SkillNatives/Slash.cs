using ProjectArena.Content.SkillNatives.Base;
using ProjectArena.Engine.ForExternalUse;
using ProjectArena.Engine.Helpers;

namespace ProjectArena.Content.SkillNatives
{
    public class Slash : INative
    {
        public void Fill(INativeManager nativeManager)
        {
            nativeManager.AddSkillNative(
                "slash",
                "slash",
                "slash",
                new[] { "damage", "target", "weapon", "melee", "physic" },
                1,
                2,
                0,
                30,
                new Targets()
                {
                    NotAllies = true,
                    Decorations = true,
                },
                true,
                (scene, owner, targetTile, skill) =>
                {
                    targetTile.DoDamageAttack(skill);
                });
        }
    }
}