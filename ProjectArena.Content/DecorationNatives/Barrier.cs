using ProjectArena.Engine.ForExternalUse;
using ProjectArena.Engine.Helpers;

namespace ProjectArena.Content.DecorationNatives
{
    public class Barrier : INative
    {
        public void Fill(INativeManager nativeManager)
        {
            nativeManager.AddDecorationNative(
                "barrier",
                "barrier",
                new string[] { "construction" },
                new TagSynergy[]
                {
                    new TagSynergy("pure", 0.5f),
                    new TagSynergy("heal", 0)
                },
                150,
                0,
                0,
                null,
                null);
        }
    }
}