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
                new[] { "damage", "target", "weapon", "melee" },
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
                4,
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
                new[] { "damage", "target", "movement", "control", "weapon", "direct" },
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
        }
    }
}