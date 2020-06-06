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
                    NotAllies = true,
                    Decorations = true,
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
                    Bearable = true,
                    Decorations = true
                },
                true,
                new[] { "DoDamageSkill" });
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
                    Bearable = true,
                    Decorations = true
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
                    Bearable = true,
                    Decorations = true
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
                    Decorations = true
                },
                true,
                new[] { "DoDamageAttack" });
            nativeManager.AddSkillNative(
                "shadowstep",
                new[] { "target", "movement", "magic", "alteration" },
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
                    Decorations = true
                },
                false,
                new[] { "MakePowerPlace" });
            nativeManager.AddSkillNative(
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
                new[] { "MakeBarrier" });
            nativeManager.AddSkillNative(
                "wand",
                new[] { "damage", "target", "weapon", "magic", "pure", "ranged", "direct" },
                4,
                2,
                0,
                20,
                new Targets()
                {
                    Allies = true,
                    NotAllies = true,
                    Decorations = true
                },
                false,
                new[] { "DoDamageSkill" });
            nativeManager.AddSkillNative(
                "mistpact",
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
                new[] { "CreateMistspawn" });
            nativeManager.AddSkillNative(
                "bloodsphere",
                new[] { "damage", "target", "magic", "blood", "ranged", "direct" },
                5,
                0,
                4,
                40,
                new Targets()
                {
                    Allies = true,
                    NotAllies = true,
                    Unbearable = true,
                    Bearable = true,
                    Decorations = true
                },
                true,
                new[] { "DoDamageSkill", "DoFixedSmallDamageSelf" });
            nativeManager.AddSkillNative(
                "offspring",
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
                new[] { "CreateOffspring", "DoDamagePercentSelf" });
            nativeManager.AddSkillNative(
                "sacrifice",
                new[] { "target", "heal", "magic", "blood" },
                1,
                0,
                0,
                -1,
                new Targets()
                {
                    Allies = true,
                    NotAllies = true,
                    Decorations = true
                },
                true,
                new[] { "DoDamageByCurrentHealth", "Sacrifice" });
            nativeManager.AddSkillNative(
                "sacrifice",
                new[] { "target", "buff", "magic", "pure" },
                4,
                6,
                4,
                0.5f,
                new Targets()
                {
                    Allies = true,
                    NotAllies = true
                },
                true,
                new[] { "Empower" });
            nativeManager.AddSkillNative(
                "mistSlash",
                new[] { "damage", "target", "weapon", "magic", "pure", "melee", "physic" },
                1,
                2,
                0,
                18,
                new Targets()
                {
                    Allies = true,
                    NotAllies = true,
                    Decorations = true,
                },
                true,
                new[] { "DoDamageAttack", "DoDamageSkill" });
            nativeManager.AddSkillNative(
                "mistShot",
                new[] { "damage", "target", "weapon", "physic", "magic", "pure", "ranged", "direct" },
                5,
                2,
                0,
                12,
                new Targets()
                {
                    Allies = true,
                    NotAllies = true,
                    Decorations = true
                },
                true,
                new[] { "DoDamageAttack", "DoDamageSkill" });
        }
    }
}