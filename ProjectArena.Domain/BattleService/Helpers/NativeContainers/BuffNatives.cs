using ProjectArena.Engine.ForExternalUse;

namespace ProjectArena.Domain.BattleService.Helpers.NativeContainers
{
    public static class BuffNatives
    {
        public static void FillBuffNatives(this INativeManager nativeManager)
        {
            nativeManager.AddBuffNative(
                "stun",
                new[] { "negative", "control" },
                false,
                1,
                false,
                false,
                1,
                0,
                new string[0],
                new[] { "Stun" },
                new string[0]);

            nativeManager.AddBuffNative(
                "tilepower",
                new[] { "positive", "offence", "tile" },
                true,
                1,
                false,
                true,
                1,
                1,
                new string[0],
                new[] { "AddStrengthMultiplier", "AddWillpowerMultiplier" },
                new string[0]);

            nativeManager.AddBuffNative(
                "empower",
                new[] { "positive", "offence" },
                false,
                1,
                false,
                false,
                2,
                1,
                new string[0],
                new[] { "AddSpellDamage", "AddAttackDamage" },
                new string[0]);
        }
    }
}