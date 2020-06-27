using ProjectArena.Content.SkillNatives.Base;
using ProjectArena.Engine.ForExternalUse;
using ProjectArena.Engine.Helpers;

namespace ProjectArena.Content.SkillNatives
{
    public class Empower : INative
    {
        public void Fill(INativeManager nativeManager)
        {
            nativeManager.AddSkillNative(
                "empower",
                "empower",
                "empower",
                new[] { "target", "buff", "magic", "pure" },
                8,
                4,
                0.5f,
                new Targets()
                {
                    Allies = true,
                    NotAllies = true,
                    Self = true
                },
                (scene, owner, targetTile, skill) =>
                {
                    var mod = owner.SkillPower;
                    targetTile.AddBuff("empower", mod * skill.Mod, 3, false);
                });
        }
    }
}