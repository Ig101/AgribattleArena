using ProjectArena.Engine.ForExternalUse;

namespace ProjectArena.Content.ActorNatives
{
    public class Enchanter : INative
    {
        public void Fill(INativeManager nativeManager)
        {
            nativeManager.AddActorNative(
                "enchanter",
                "enchanter",
                "blacksmith",
                new[] { "organic", "intelligent" },
                0,
                new Engine.Helpers.TagSynergy[0]);
        }
    }
}