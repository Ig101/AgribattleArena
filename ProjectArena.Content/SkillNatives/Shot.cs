using ProjectArena.Content.SkillNatives.Base;
using ProjectArena.Engine.ForExternalUse;
using ProjectArena.Engine.Helpers;

namespace ProjectArena.Content.SkillNatives
{
    public class Shot : INative
    {
        public void Fill(INativeManager nativeManager)
        {
            nativeManager.AddSkillNative(
                "shot",
                "shot",
                "shot",
                new[] { "damage", "target", "weapon", "physic", "ranged", "direct" },
                8,
                3,
                0,
                25,
                new Targets()
                {
                    NotAllies = true,
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