using ProjectArena.Engine.ForExternalUse;

namespace ProjectArena.Domain.BattleService.Helpers.NativeContainers
{
    public static class TileNatives
    {
        public static void FillTileNatives(this INativeManager nativeManager)
        {
            nativeManager.AddTileNative(
                "grass",
                new string[] { "natural" },
                false,
                0,
                false,
                1,
                new string[0],
                new string[0]);
            nativeManager.AddTileNative(
                "powerplace",
                new string[] { "magic", "positive" },
                false,
                0,
                false,
                1,
                new string[0],
                new string[] { "AddTilePower" });
        }
    }
}