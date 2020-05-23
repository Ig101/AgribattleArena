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
            nativeManager.AddActorNative(
                "architect",
                new[] { "organic", "intelligent" },
                0,
                new Engine.Helpers.TagSynergy[0]);
            nativeManager.AddActorNative(
                "bloodletter",
                new[] { "organic", "intelligent" },
                0,
                new Engine.Helpers.TagSynergy[0]);
            nativeManager.AddActorNative(
                "enchanter",
                new[] { "organic", "intelligent" },
                0,
                new Engine.Helpers.TagSynergy[0]);
            nativeManager.AddActorNative(
                "fighter",
                new[] { "organic", "intelligent" },
                0,
                new Engine.Helpers.TagSynergy[0]);
            nativeManager.AddActorNative(
                "mistcaller",
                new[] { "organic", "intelligent" },
                0,
                new Engine.Helpers.TagSynergy[0]);
            nativeManager.AddActorNative(
                "ranger",
                new[] { "organic", "intelligent" },
                0,
                new Engine.Helpers.TagSynergy[0]);
        }
    }
}