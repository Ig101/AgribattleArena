using ProjectArena.Engine.ForExternalUse;

namespace ProjectArena.Content.ActorNatives
{
    public class Architect : INative
    {
        public void Fill(INativeManager nativeManager)
        {
            nativeManager.AddActorNative(
                "architect",
                "architect",
                "carver",
                new[] { "organic", "intelligent" },
                0,
                new Engine.Helpers.TagSynergy[0]);
        }
    }
}