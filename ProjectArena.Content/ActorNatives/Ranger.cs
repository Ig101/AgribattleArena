using ProjectArena.Engine.ForExternalUse;

namespace ProjectArena.Content.ActorNatives
{
    public class Ranger : INative
    {
        public void Fill(INativeManager nativeManager)
        {
            nativeManager.AddActorNative(
                "offspring",
                new[] { "organic", "controlled" },
                0,
                new Engine.Helpers.TagSynergy[0]);
        }
    }
}