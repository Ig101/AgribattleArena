using ProjectArena.Engine.ForExternalUse;
using ProjectArena.Engine.Helpers;

namespace ProjectArena.Domain.BattleService.Helpers.NativeContainers
{
    public static class DecorationNatives
    {
        public static void FillDecorationNatives(this INativeManager nativeManager)
        {
            nativeManager.AddDecorationNative(
                "barrier",
                new string[] { "construction" },
                new TagSynergy[]
                {
                    new TagSynergy("pure", 0.5f),
                    new TagSynergy("heal", 0)
                },
                100,
                0,
                0,
                new string[0],
                new string[0]);
        }
    }
}