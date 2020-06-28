using System;
using ProjectArena.Engine;
using ProjectArena.Engine.ForExternalUse;
using ProjectArena.Engine.Helpers;
using ProjectArena.Engine.Natives;

namespace ProjectArena.Content.SkillNatives
{
    public class Offspring : INative
    {
        public void Fill(INativeManager nativeManager)
        {
            nativeManager.AddSkillNative(
                "offspring",
                "offspring",
                "mistoffspring",
                new[] { "target", "summon", "magic", "blood" },
                1,
                2,
                4,
                0.2f,
                new Targets()
                {
                    Bearable = true
                },
                false,
                (scene, owner, targetTile, skill) =>
                {
                    var mod = (int)Math.Ceiling(owner.Constitution * skill.Mod);
                    scene.CreateActor(
                        owner.Owner as Player,
                        null,
                        "offspring",
                        new RoleModelNative(
                            scene.NativeManager,
                            "offspring",
                            mod,
                            2,
                            mod,
                            30,
                            2,
                            "slash",
                            new string[]
                            {
                                "sacrifice"
                            }),
                        targetTile,
                        null,
                        null,
                        null);
                    owner.Damage(mod * scene.VarManager.ConstitutionMod, skill.AggregatedTags);
                });
        }
    }
}