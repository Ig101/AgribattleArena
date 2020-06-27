using ProjectArena.Engine.ForExternalUse;

namespace ProjectArena.Content.ActorNatives
{
    public class Offspring : INative
    {
        public void Fill(INativeManager nativeManager)
        {
            nativeManager.AddActorNative(
                "offspring",
                "offspring",
                "mistoffspring",
                new[] { "organic", "controlled" },
                0,
                new Engine.Helpers.TagSynergy[0],
                null,
                null,
                null);
        }
    }
}