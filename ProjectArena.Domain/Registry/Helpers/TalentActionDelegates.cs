using System.Collections.Generic;
using System.Linq;
using ProjectArena.Domain.Registry.Entities;
using ProjectArena.Engine.ForExternalUse.Generation.ObjectInterfaces;

namespace ProjectArena.Domain.Registry.Helpers
{
    public static class TalentActionDelegates
    {
        public delegate void Action(
            ref string attackSkill,
            ref int actionPointsIncome,
            ref ICollection<string> skills,
            ref ICollection<string> startBuffs);

        public static void LearnMistWeapon(
            ref string attackSkill,
            ref int actionPointsIncome,
            ref ICollection<string> skills,
            ref ICollection<string> startBuffs)
        {
            attackSkill = attackSkill switch
            {
                "shot" => "mistShot",
                _ => "mistSlash",
            };
        }

        public static void LearnCharge(
            ref string attackSkill,
            ref int actionPointsIncome,
            ref ICollection<string> skills,
            ref ICollection<string> startBuffs)
        {
            skills.Add("charge");
        }

        public static void LearnWarden(
            ref string attackSkill,
            ref int actionPointsIncome,
            ref ICollection<string> skills,
            ref ICollection<string> startBuffs)
        {
            skills.Remove("magicMissle");
            skills.Add("warden");
        }

        public static void LearnEmpower(
            ref string attackSkill,
            ref int actionPointsIncome,
            ref ICollection<string> skills,
            ref ICollection<string> startBuffs)
        {
            skills.Add("empower");
        }

        public static void LearnMarksmanship(
            ref string attackSkill,
            ref int actionPointsIncome,
            ref ICollection<string> skills,
            ref ICollection<string> startBuffs)
        {
            attackSkill = attackSkill switch
            {
                "mistSlash" => "mistShot",
                _ => "shot",
            };
        }

        public static void LearnOffspring(
            ref string attackSkill,
            ref int actionPointsIncome,
            ref ICollection<string> skills,
            ref ICollection<string> startBuffs)
        {
            skills.Add("offspring");
        }

        public static void LearnMistwalk(
            ref string attackSkill,
            ref int actionPointsIncome,
            ref ICollection<string> skills,
            ref ICollection<string> startBuffs)
        {
            skills.Add("mistwalk");
        }

        public static void LearnBloodsphere(
            ref string attackSkill,
            ref int actionPointsIncome,
            ref ICollection<string> skills,
            ref ICollection<string> startBuffs)
        {
            skills.Remove("magicMissle");
            skills.Add("bloodsphere");
        }

        public static void LearnBarrier(
            ref string attackSkill,
            ref int actionPointsIncome,
            ref ICollection<string> skills,
            ref ICollection<string> startBuffs)
        {
            skills.Add("barrier");
        }

        public static void LearnMistPact(
            ref string attackSkill,
            ref int actionPointsIncome,
            ref ICollection<string> skills,
            ref ICollection<string> startBuffs)
        {
            skills.Add("mistpact");
        }

        public static void LearnWand(
            ref string attackSkill,
            ref int actionPointsIncome,
            ref ICollection<string> skills,
            ref ICollection<string> startBuffs)
        {
            attackSkill = "wand";
        }

        public static void LearnPowerplace(
            ref string attackSkill,
            ref int actionPointsIncome,
            ref ICollection<string> skills,
            ref ICollection<string> startBuffs)
        {
            skills.Add("powerplace");
        }
    }
}