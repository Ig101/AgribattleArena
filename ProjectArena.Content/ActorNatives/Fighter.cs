using ProjectArena.Engine.ForExternalUse;

namespace ProjectArena.Content.ActorNatives
{
    public class Fighter : INative
    {
        public void Fill(INativeManager nativeManager)
        {
            nativeManager.AddActorNative(
                "fighter",
                "fighter",
                "fencer",
                new[] { "organic", "intelligent" },
                0,
                new Engine.Helpers.TagSynergy[0],
                null,
                null,
                null);
        }
    }
}