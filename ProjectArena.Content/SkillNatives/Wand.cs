using ProjectArena.Content.SkillNatives.Base;
using ProjectArena.Engine.ForExternalUse;
using ProjectArena.Engine.Helpers;

namespace ProjectArena.Content.SkillNatives
{
    public class Wand : INative
    {
        public void Fill(INativeManager nativeManager)
        {
            nativeManager.AddSkillNative(
                "wand",
                "wand",
                "wand",
                new[] { "damage", "target", "weapon", "magic", "pure", "ranged", "direct" },
                4,
                3,
                0,
                25,
                new Targets()
                {
                    NotAllies = true,
                    Decorations = true
                },
                false,
                (scene, owner, targetTile, skill) =>
                {
                    targetTile.DoDamageSkill(skill);
                });
        }
    }
}