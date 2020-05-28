using ProjectArena.Engine.ForExternalUse;
using ProjectArena.Engine.Helpers;

namespace ProjectArena.Domain.BattleService.Helpers.NativeContainers
{
    public static class SkillNatives
    {
        public static void FillSkillNatives(this INativeManager nativeManager)
        {
            nativeManager.AddSkillNative(
                "slash",
                new[] { "damage", "target", "weapon", "melee", "physic" },
                1,
                2,
                0,
                30,
                new Targets()
                {
                    Allies = true,
                    NotAllies = true
                },
                true,
                new[] { "DoDamageAttack" });
            nativeManager.AddSkillNative(
                "magicMissle",
                new[] { "damage", "target", "magic", "pure", "ranged", "direct" },
                5,
                3,
                4,
                40,
                new Targets()
                {
                    Allies = true,
                    NotAllies = true,
                    Unbearable = true,
                    Bearable = true
                },
                true,
                new[] { "DoDamageAttack" });
            nativeManager.AddSkillNative(
                "charge",
                new[] { "damage", "target", "movement", "control", "weapon", "direct", "physic" },
                4,
                4,
                3,
                10,
                new Targets()
                {
                    Allies = true,
                    NotAllies = true,
                    Bearable = true
                },
                true,
                new[] { "DoDamageAttack", "DoChargeStun", "DoChargeMove" });
            nativeManager.AddSkillNative(
                "warden",
                new[] { "damage", "target", "physic", "ranged", "direct" },
                3,
                3,
                4,
                50,
                new Targets()
                {
                    Allies = true,
                    NotAllies = true,
                    Unbearable = true,
                    Bearable = true
                },
                true,
                new[] { "DoDamageAttack" });
            nativeManager.AddSkillNative(
                "shot",
                new[] { "damage", "target", "weapon", "physic", "ranged", "direct" },
                5,
                2,
                0,
                20,
                new Targets()
                {
                    Allies = true,
                    NotAllies = true,
                },
                true,
                new[] { "DoDamageAttack" });
            nativeManager.AddSkillNative(
                "shadowstep",
                new[] { "target", "movement" },
                5,
                1,
                5,
                0,
                new Targets()
                {
                    Bearable = true
                },
                false,
                new[] { "DoMove" });
            nativeManager.AddSkillNative(
                "powerplace",
                new[] { "target", "tile", "buff" },
                4,
                6,
                2,
                0,
                new Targets()
                {
                    Bearable = true,
                    Allies = true,
                    NotAllies = true
                },
                false,
                new[] { "MakePowerPlace" });
        }
    }
}