using ProjectArena.Engine.ForExternalUse;

namespace ProjectArena.Content.ActorNatives
{
    public class Mistspawn : INative
    {
        public void Fill(INativeManager nativeManager)
        {
            nativeManager.AddActorNative(
                "mistspawn",
                "tamedspawn",
                "lesserspawn",
                new[] { "organic", "intelligent", "controlled" },
                0,
                new Engine.Helpers.TagSynergy[0]);
        }
    }
}