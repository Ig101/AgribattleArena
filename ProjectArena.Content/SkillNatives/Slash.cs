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
                0,
                30,
                new Targets()
                {
                    Allies = true,
                    NotAllies = true,
                    Decorations = true,
                },
                (scene, owner, targetTile, skill) =>
                {
                    targetTile.DoDamageAttack(skill);
                });
        }
    }
}