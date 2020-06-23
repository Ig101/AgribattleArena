using ProjectArena.Engine.ForExternalUse;

namespace ProjectArena.Content.ActorNatives
{
    public class Ranger : INative
    {
        public void Fill(INativeManager nativeManager)
        {
            nativeManager.AddActorNative(
                "ranger",
                "ranger",
                "impaler",
                new[] { "organic", "intelligent" },
                0,
                new Engine.Helpers.TagSynergy[0]);
        }
    }
}