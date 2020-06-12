using ProjectArena.Engine.ForExternalUse;

namespace ProjectArena.Content.ActorNatives
{
    public class Offspring : INative
    {
        public void Fill(INativeManager nativeManager)
        {
            nativeManager.AddActorNative(
                "mistspawn",
                new[] { "organic", "intelligent", "controlled" },
                0,
                new Engine.Helpers.TagSynergy[0]);
        }
    }
}