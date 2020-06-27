using ProjectArena.Content.SkillNatives.Base;
using ProjectArena.Engine.ForExternalUse;
using ProjectArena.Engine.Helpers;

namespace ProjectArena.Content.SkillNatives
{
    public class MistSlash : INative
    {
        public void Fill(INativeManager nativeManager)
        {
            nativeManager.AddSkillNative(
                "mistSlash",
                "mistSlash",
                "mistSlash",
                new[] { "damage", "target", "weapon", "magic", "pure", "melee", "physic" },
                1,
                0,
                25,
                new Targets()
                {
                    Allies = true,
                    NotAllies = true,
                    Decorations = true,
                },
                (scene, owner, targetTile, skill) =>
                {
                    targetTile.DoDamageAttack(skill);
                    targetTile.DoDamageSkill(skill);
                });
        }
    }
}