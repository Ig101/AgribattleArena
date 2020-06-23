using ProjectArena.Engine.ForExternalUse;

namespace ProjectArena.Content.ActorNatives
{
    public class Bloodletter : INative
    {
        public void Fill(INativeManager nativeManager)
        {
            nativeManager.AddActorNative(
                "bloodletter",
                "bloodletter",
                "ritualist",
                new[] { "organic", "intelligent" },
                0,
                new Engine.Helpers.TagSynergy[0]);
        }
    }
}