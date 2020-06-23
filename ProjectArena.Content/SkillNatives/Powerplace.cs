using ProjectArena.Engine.ForExternalUse;
using ProjectArena.Engine.Helpers;

namespace ProjectArena.Content.SkillNatives
{
    public class Powerplace : INative
    {
        public void Fill(INativeManager nativeManager)
        {
           nativeManager.AddSkillNative(
                "powerplace",
                "powerplace",
                "powerplace",
                new[] { "target", "tile", "buff", "magic", "alteration" },
                4,
                6,
                2,
                0,
                new Targets()
                {
                    Bearable = true,
                    Allies = true,
                    NotAllies = true,
                    Decorations = true,
                    Self = true
                },
                false,
                (scene, owner, targetTile, skill) =>
                {
                    scene.ChangeTile("powerplace", targetTile.X, targetTile.Y, targetTile.Height, owner.Owner);
                });
        }
    }
}