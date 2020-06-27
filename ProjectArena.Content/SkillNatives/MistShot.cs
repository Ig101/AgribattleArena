using ProjectArena.Content.SkillNatives.Base;
using ProjectArena.Engine.ForExternalUse;
using ProjectArena.Engine.Helpers;

namespace ProjectArena.Content.SkillNatives
{
    public class MistShot : INative
    {
        public void Fill(INativeManager nativeManager)
        {
            nativeManager.AddSkillNative(
                "mistShot",
                "mistShot",
                "mistShot",
                new[] { "damage", "target", "weapon", "physic", "magic", "pure", "ranged", "direct" },
                5,
                0,
                17,
                new Targets()
                {
                    Allies = true,
                    NotAllies = true,
                    Decorations = true
                },
                (scene, owner, targetTile, skill) =>
                {
                    targetTile.DoDamageAttack(skill);
                    targetTile.DoDamageSkill(skill);
                });
        }
    }
}