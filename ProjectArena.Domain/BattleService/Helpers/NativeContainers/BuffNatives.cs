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
                1,
                0,
                new string[0],
                new[] { "Stun" },
                new string[0]);
        }
    }
}