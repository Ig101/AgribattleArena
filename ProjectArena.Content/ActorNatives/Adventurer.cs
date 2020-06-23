using ProjectArena.Engine.ForExternalUse;

namespace ProjectArena.Content.ActorNatives
{
    public class Adventurer : INative
    {
        public void Fill(INativeManager nativeManager)
        {
            nativeManager.AddActorNative(
                "adventurer",
                "adventurer",
                "spawn",
                new[] { "organic", "intelligent" },
                0,
                new Engine.Helpers.TagSynergy[0]);
        }
    }
}