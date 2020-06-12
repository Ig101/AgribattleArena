using ProjectArena.Engine.ForExternalUse;

namespace ProjectArena.Content.BuffNatives
{
    public class Tilepower : INative
    {
        public void Fill(INativeManager nativeManager)
        {
            nativeManager.AddBuffNative(
                "tilepower",
                new[] { "positive", "offence", "tile" },
                true,
                1,
                false,
                true,
                null,
                1,
                null,
                (manager, buff) =>
                {
                    manager.AdditionStrength += manager.Parent.SelfStrength * buff.Mod;
                    manager.AdditionWillpower += manager.Parent.SelfWillpower * buff.Mod;
                },
                null);
        }
    }
}