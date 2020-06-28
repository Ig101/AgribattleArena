using ProjectArena.Content.SkillNatives.Base;
using ProjectArena.Engine.ForExternalUse;
using ProjectArena.Engine.Helpers;

namespace ProjectArena.Content.SkillNatives
{
    public class BloodSphere : INative
    {
        public void Fill(INativeManager nativeManager)
        {
            nativeManager.AddSkillNative(
                "bloodsphere",
                "bloodsphere",
                "mistsphere",
                new[] { "damage", "target", "magic", "blood", "ranged", "direct" },
                8,
                3,
                0,
                40,
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
                    owner.Damage(10, skill.AggregatedTags);
                    targetTile.DoDamageSkill(skill);
                });
        }
    }
}