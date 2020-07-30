using ProjectArena.Engine.ForExternalUse;
using ProjectArena.Engine.Helpers;

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
                true,
                null,
                null);
            nativeManager.AddTileNative(
                "rock",
                new string[] { "natural" },
                false,
                0,
                true,
                1,
                true,
                null,
                null);
            nativeManager.AddDecorationNative(
                "tree",
                "tree",
                new string[] { "natural", "flammable" },
                new TagSynergy[0],
                500,
                0,
                0,
                null,
                null);
        }
    }
}