using ProjectArena.Engine.ForExternalUse;

namespace ProjectArena.Content.BuffNatives
{
    public class Stun : INative
    {
        public void Fill(INativeManager nativeManager)
        {
            nativeManager.AddBuffNative(
                "stun",
                new[] { "negative", "control" },
                false,
                1,
                false,
                1,
                0,
                (manager, buff) =>
                {
                    manager.CanMove = false;
                    manager.CanAct = false;
                },
                null);
        }
    }
}