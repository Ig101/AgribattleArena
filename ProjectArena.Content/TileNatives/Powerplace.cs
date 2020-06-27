using ProjectArena.Content.SkillNatives.Base;
using ProjectArena.Engine.ForExternalUse;

namespace ProjectArena.Content.TileNatives
{
    public class Powerplace : INative
    {
        public void Fill(INativeManager nativeManager)
        {
            nativeManager.AddTileNative(
                "powerplace",
                new string[] { "magic", "positive" },
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