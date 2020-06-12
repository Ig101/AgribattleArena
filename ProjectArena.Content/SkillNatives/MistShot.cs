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
                new[] { "damage", "target", "weapon", "physic", "magic", "pure", "ranged", "direct" },
                5,
                3,
                0,
                17,
                new Targets()
                {
                    Allies = true,
                    NotAllies = true,
                    Decorations = true
                },
                true,
                (scene, owner, targetTile, skill) =>
                {
                    targetTile.DoDamageAttack(skill);
                    targetTile.DoDamageSkill(skill);
                });
        }
    }
}