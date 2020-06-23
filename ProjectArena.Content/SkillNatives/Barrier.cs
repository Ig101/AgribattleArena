using ProjectArena.Engine;
using ProjectArena.Engine.ForExternalUse;
using ProjectArena.Engine.Helpers;

namespace ProjectArena.Content.SkillNatives
{
    public class Barrier : INative
    {
        public void Fill(INativeManager nativeManager)
        {
           nativeManager.AddSkillNative(
                "barrier",
                "barrier",
                "barrier",
                new[] { "target", "decoration", "magic", "alteration" },
                4,
                4,
                0,
                0,
                new Targets()
                {
                    Bearable = true
                },
                false,
                (scene, owner, targetTile, skill) =>
                {
                    scene.CreateDecoration(owner.Owner as Player, "barrier", targetTile, null, null, null, null, null);
                });
        }
    }
}