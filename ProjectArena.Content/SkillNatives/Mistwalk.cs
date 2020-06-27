using ProjectArena.Engine.ForExternalUse;
using ProjectArena.Engine.Helpers;

namespace ProjectArena.Content.SkillNatives
{
    public class Mistwalk : INative
    {
        public void Fill(INativeManager nativeManager)
        {
            nativeManager.AddSkillNative(
                "mistwalk",
                "mistwalk",
                "mistwalk",
                new[] { "target", "movement", "magic", "alteration" },
                5,
                5,
                0,
                new Targets()
                {
                    Bearable = true
                },
                (scene, owner, targetTile, skill) =>
                {
                    owner.ChangePosition(targetTile);
                });
        }
    }
}