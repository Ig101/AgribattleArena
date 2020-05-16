using ProjectArena.Engine.ForExternalUse;

namespace ProjectArena.Domain.BattleService.Helpers.NativeContainers
{
    public static class ActorNatives
    {
        public static void FillActorNatives(this INativeManager nativeManager)
        {
            nativeManager.AddActorNative(
                "adventurer",
                new[] { "organic", "intelligent" },
                0,
                new Engine.Helpers.TagSynergy[0]);
        }
    }
}