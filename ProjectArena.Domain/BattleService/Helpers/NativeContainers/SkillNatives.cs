using ProjectArena.Engine.ForExternalUse;

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
                true,
                new[] { "DoDamageAttack" });
            nativeManager.AddSkillNative(
                "explosion",
                new[] { "damage", "area", "magic", "fire", "range" },
                3,
                4,
                3,
                20,
                false,
                new[] { "DoSmallAoeDamageSkill", "DoSmallAoeOneTurnStun" });
        }
    }
}