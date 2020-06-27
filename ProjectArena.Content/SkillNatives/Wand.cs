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
                0,
                20,
                new Targets()
                {
                    Allies = true,
                    NotAllies = true,
                    Decorations = true
                },
                (scene, owner, targetTile, skill) =>
                {
                    targetTile.DoDamageSkill(skill);
                });
        }
    }
}