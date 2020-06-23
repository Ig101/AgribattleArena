using ProjectArena.Engine;
using ProjectArena.Engine.ForExternalUse;
using ProjectArena.Engine.Helpers;

namespace ProjectArena.Content.SkillNatives
{
    public class MistPact : INative
    {
        public void Fill(INativeManager nativeManager)
        {
            nativeManager.AddSkillNative(
                "mistpact",
                "mistpact",
                "mistsummon",
                new[] { "target", "summon", "magic", "pure" },
                1,
                2,
                8,
                1,
                new Targets()
                {
                    Bearable = true
                },
                false,
                (scene, owner, targetTile, skill) =>
                {
                    scene.CreateActor(owner.Owner as Player, "mistspawn", "mistspawn", targetTile, null, null, null);
                });
        }
    }
}