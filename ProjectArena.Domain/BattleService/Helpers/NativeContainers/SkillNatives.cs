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
                new[] { "damage", "target", "slash", "melee" },
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
                "explosion",
                new[] { "damage", "area", "magic", "fire", "range" },
                3,
                4,
                3,
                20,
                new Targets()
                {
                    Allies = true,
                    NotAllies = true,
                    Unbearable = true,
                    Bearable = true
                },
                false,
                new[] { "DoSmallAoeDamageSkill", "DoSmallAoeOneTurnStun" });
        }
    }
}