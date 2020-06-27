using ProjectArena.Engine.ForExternalUse;

namespace ProjectArena.Content.TileNatives
{
    public class Grass : INative
    {
        public void Fill(INativeManager nativeManager)
        {
            nativeManager.AddTileNative(
                "grass",
                new string[] { "natural" },
                false,
                0,
                false,
                1,
                null,
                null,
                null);
        }
    }
}