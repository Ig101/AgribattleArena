using ProjectArena.Engine.ForExternalUse;

namespace ProjectArena.Content.BuffNatives
{
    public class Empower : INative
    {
        public void Fill(INativeManager nativeManager)
        {
            nativeManager.AddBuffNative(
                "empower",
                new[] { "positive", "offence" },
                false,
                1,
                false,
                false,
                2,
                1,
                null,
                (manager, buff) =>
                {
                    manager.AttackPower += buff.Mod;
                    manager.SkillPower += buff.Mod;
                },
                null);
        }
    }
}