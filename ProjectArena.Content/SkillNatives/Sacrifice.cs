using ProjectArena.Engine.ForExternalUse;
using ProjectArena.Engine.Helpers;

namespace ProjectArena.Content.SkillNatives
{
    public class Sacrifice : INative
    {
        public void Fill(INativeManager nativeManager)
        {
            nativeManager.AddSkillNative(
                "sacrifice",
                "sacrifice",
                "mistsacrifice",
                new[] { "target", "heal", "magic", "blood" },
                1,
                0,
                -1,
                new Targets()
                {
                    Allies = true,
                    NotAllies = true,
                    Decorations = true
                },
                (scene, owner, targetTile, skill) =>
                {
                    targetTile.TempObject.Damage(skill.Mod * owner.DamageModel.Health, skill.AggregatedTags);
                    owner.Damage(owner.MaxHealth, new string[0]);
                });
        }
    }
}