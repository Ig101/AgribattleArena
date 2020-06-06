using System;
using System.Collections.Generic;
using System.Linq;
using ProjectArena.Domain.Registry.Entities;
using ProjectArena.Domain.Registry.Helpers;
using ProjectArena.Infrastructure.Enums;

namespace ProjectArena.Domain.Registry.Migrations
{
    public class M0000_AddDefaultTalents : IContentMigration
    {
        private Dictionary<string, (string name, int strength, int willpower, int constitution, int speed)> _defaultNodes;

        public M0000_AddDefaultTalents()
        {
            _defaultNodes = new Dictionary<string, (string name, int strength, int willpower, int constitution, int speed)>()
            {
                { "Sw", (name: "Cruelty", strength: 1, willpower: -1, constitution: 0, speed: 0) },
                { "Si", (name: "Power", strength: 1, willpower: 0, constitution: 0, speed: -1) },
                { "Sc", (name: "Might", strength: 1, willpower: 0, constitution: -1, speed: 0) },
                { "Ws", (name: "Focus", strength: -1, willpower: 1, constitution: 0, speed: 0) },
                { "Wi", (name: "Will", strength: 0, willpower: 1, constitution: 0, speed: -1) },
                { "Wc", (name: "Zealotry", strength: 0, willpower: 1, constitution: -1, speed: 0) },
                { "Is", (name: "Quickness", strength: -1, willpower: 0, constitution: 0, speed: 1) },
                { "Iw", (name: "Dexterity", strength: 0, willpower: -1, constitution: 0, speed: 1) },
                { "Ic", (name: "Agility", strength: 0, willpower: 0, constitution: -1, speed: 1) },
                { "Cs", (name: "Endurance", strength: -1, willpower: 0, constitution: 1, speed: 0) },
                { "Cw", (name: "Toughness", strength: 0, willpower: -1, constitution: 1, speed: 0) },
                { "Ci", (name: "Resilience", strength: 0, willpower: 0, constitution: 1, speed: -1) }
            };
        }

        public TalentNode NewDefaultNode(string name, int position)
        {
            return new TalentNode()
            {
                Position = position,
                Id = name,
                Name = _defaultNodes[name].name,
                Class = null,
                ClassPoints = 0,
                UniqueDescription = null,
                UniqueAction = null,
                StrengthModifier = _defaultNodes[name].strength,
                WillpowerModifier = _defaultNodes[name].willpower,
                ConstitutionModifier = _defaultNodes[name].constitution,
                SpeedModifier = _defaultNodes[name].speed,
                Prerequisites = new string[0],
                Exceptions = new string[0]
            };
        }

        public void Up(RegistryContext context)
        {
            context.TalentMap.ReplaceOne(
                x => x.Position == 10011,
                NewDefaultNode("Si", 10011),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 10012,
                NewDefaultNode("Si", 10012),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 11012,
                NewDefaultNode("Ci", 11012),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 11013,
                NewDefaultNode("Sw", 11013),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 12012,
                NewDefaultNode("Ci", 12012),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 13012,
                NewDefaultNode("Sw", 13012),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 13011,
                NewDefaultNode("Sw", 13011),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 13010,
                NewDefaultNode("Si", 13010),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 14007,
                NewDefaultNode("Wi", 14007),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 14008,
                NewDefaultNode("Wi", 14008),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 14009,
                NewDefaultNode("Wi", 14009),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 14010,
                NewDefaultNode("Si", 14010),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 14011,
                new TalentNode()
                {
                    Position = 14011,
                    Id = "mistweapon",
                    Name = "Mist Weapon",
                    Class = CharacterClass.Enchanter,
                    ClassPoints = 1,
                    UniqueDescription = "Increases base attack damage. 50% damage of attack converts to willpower-based damage. Works with \"Marksman\".",
                    UniqueAction = "LearnMistWeapon",
                    StrengthModifier = 0,
                    WillpowerModifier = 0,
                    ConstitutionModifier = 0,
                    SpeedModifier = 0,
                    Prerequisites = new string[0],
                    Exceptions = new string[] { "wand" },
                    SkillsAmount = 0
                },
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 14012,
                NewDefaultNode("Sw", 14012),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 14013,
                NewDefaultNode("Cw", 14013),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 14014,
                NewDefaultNode("Cw", 14014),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 14015,
                NewDefaultNode("Ic", 14015),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 14016,
                NewDefaultNode("Ic", 14016),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 14017,
                NewDefaultNode("Ic", 14017),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 15006,
                NewDefaultNode("Sc", 15006),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 15007,
                NewDefaultNode("Sc", 15007),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 15010,
                NewDefaultNode("Sw", 15010),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 15014,
                NewDefaultNode("Cw", 15014),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 15017,
                NewDefaultNode("Sw", 15017),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 15018,
                NewDefaultNode("Sw", 15018),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 16005,
                NewDefaultNode("Wc", 16005),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 16006,
                NewDefaultNode("Sc", 16006),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 16009,
                NewDefaultNode("Si", 16009),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 16010,
                NewDefaultNode("Si", 16010),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 16011,
                NewDefaultNode("Ci", 16011),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 16013,
                NewDefaultNode("Sw", 16013),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 16014,
                NewDefaultNode("Sw", 16014),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 16018,
                NewDefaultNode("Sw", 16018),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 16019,
                NewDefaultNode("Ic", 16019),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 17005,
                NewDefaultNode("Wc", 17005),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 17008,
                NewDefaultNode("Sc", 17008),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 17009,
                NewDefaultNode("Si", 17009),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 17011,
                NewDefaultNode("Ci", 17011),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 17013,
                new TalentNode
                {
                    Position = 17013,
                    Id = "charge",
                    Name = "Charge",
                    Class = CharacterClass.Fighter,
                    ClassPoints = 1,
                    UniqueDescription = "Learns the \"Charge\" ability. Charges to the position within a range of 4 and deal small damage to target. Stun for one turn if range is two or more. Costs 4 action points. Recharges in 3 turns.",
                    UniqueAction = "LearnCharge",
                    StrengthModifier = -5,
                    WillpowerModifier = -3,
                    ConstitutionModifier = 0,
                    SpeedModifier = 0,
                    Prerequisites = new string[0],
                    Exceptions = new string[0],
                    SkillsAmount = 1
                },
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 17014,
                NewDefaultNode("Sw", 17014),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 17015,
                NewDefaultNode("Sc", 17015),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 17016,
                NewDefaultNode("Sc", 17016),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 17017,
                NewDefaultNode("Sc", 17017),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 17018,
                new TalentNode()
                {
                    Position = 17018,
                    Id = "warden",
                    Name = "Warden",
                    Class = CharacterClass.Fighter,
                    ClassPoints = 1,
                    UniqueDescription = "Learns the \"Warden\" ability. Replaces \"Magic missle\" with shield throw that deals increased strength-based damage within a range of 3.",
                    UniqueAction = "LearnWarden",
                    StrengthModifier = 0,
                    WillpowerModifier = 0,
                    ConstitutionModifier = 0,
                    SpeedModifier = 0,
                    Prerequisites = new string[0],
                    Exceptions = new string[] { "bloodsphere" },
                    SkillsAmount = 0
                },
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 17019,
                NewDefaultNode("Ic", 17019),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 18005,
                NewDefaultNode("Wc", 18005),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 18008,
                NewDefaultNode("Sc", 18008),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 18011,
                NewDefaultNode("Ci", 18011),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 18013,
                NewDefaultNode("Sw", 18013),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 18015,
                NewDefaultNode("Ic", 18015),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 18019,
                NewDefaultNode("Ic", 18019),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 19005,
                NewDefaultNode("Si", 19005),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 19006,
                NewDefaultNode("Si", 19006),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 19007,
                NewDefaultNode("Si", 19007),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 19008,
                NewDefaultNode("Sc", 19008),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 19011,
                NewDefaultNode("Cw", 19011),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 19013,
                NewDefaultNode("Sw", 19013),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 19015,
                NewDefaultNode("Ic", 19015),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 19019,
                NewDefaultNode("Iw", 19019),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 20005,
                NewDefaultNode("Wi", 20005),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 20008,
                new TalentNode()
                {
                    Position = 20008,
                    Id = "empower",
                    Name = "Empower",
                    Class = CharacterClass.Enchanter,
                    ClassPoints = 1,
                    UniqueDescription = "Learns the \"Empower\" ability. Increases target's damage by 50% of willpower modifier for 3 turns. Costs 6 action points. Recharges in 4 turns.",
                    UniqueAction = "LearnEmpower",
                    StrengthModifier = -5,
                    WillpowerModifier = 0,
                    ConstitutionModifier = -5,
                    SpeedModifier = 0,
                    Prerequisites = new string[0],
                    Exceptions = new string[0],
                    SkillsAmount = 1
                },
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 20009,
                NewDefaultNode("Wc", 20009),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 20011,
                NewDefaultNode("Cw", 20011),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 20012,
                new TalentNode()
                {
                    Position = 20012,
                    Id = "fighter",
                    Name = "Brawling",
                    Class = CharacterClass.Fighter,
                    ClassPoints = 1,
                    UniqueDescription = "Fight for your life. That is what only matters.",
                    UniqueAction = null,
                    StrengthModifier = 4,
                    WillpowerModifier = -4,
                    ConstitutionModifier = 4,
                    SpeedModifier = -4,
                    Prerequisites = new string[0],
                    Exceptions = new string[0]
                },
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 20013,
                NewDefaultNode("Sw", 20013),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 20015,
                NewDefaultNode("Ic", 20015),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 20019,
                NewDefaultNode("Iw", 20019),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 21004,
                NewDefaultNode("Wi", 21004),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 21005,
                NewDefaultNode("Wi", 21005),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 21009,
                NewDefaultNode("Wc", 21009),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 21010,
                NewDefaultNode("Wc", 21010),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 21012,
                NewDefaultNode("Ci", 21012),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 21014,
                NewDefaultNode("Sc", 21014),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 21015,
                NewDefaultNode("Sc", 21015),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 21017,
                new TalentNode()
                {
                    Position = 21017,
                    Id = "marksmanship",
                    Name = "Marksmanship",
                    Class = CharacterClass.Ranger,
                    ClassPoints = 1,
                    UniqueDescription = "Replaces attack with the range one that deals slightly less damage within a range of 4.",
                    UniqueAction = "LearnMarksmanship",
                    StrengthModifier = 0,
                    WillpowerModifier = 0,
                    ConstitutionModifier = 0,
                    SpeedModifier = 0,
                    Prerequisites = new string[0],
                    Exceptions = new string[] { "wand" },
                    SkillsAmount = 0
                },
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 21018,
                NewDefaultNode("Sc", 21018),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 21019,
                NewDefaultNode("Iw", 21019),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 22004,
                NewDefaultNode("Ws", 22004),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 22005,
                new TalentNode()
                {
                    Position = 22005,
                    Id = "offspring",
                    Name = "Offspring",
                    Class = CharacterClass.Bloodletter,
                    ClassPoints = 1,
                    UniqueDescription = "Learns the \"Offspring\" ability. Summons a spawn with 20% of caster's health nearby and reduces health of the caster by that amount. Strength depends on the amount of sacrificed health. Can be sacrificed to heal organic object nearby by its amount of health. Costs 2 action points. Recharges in 4 turns.",
                    UniqueAction = "LearnOffspring",
                    StrengthModifier = 0,
                    WillpowerModifier = -5,
                    ConstitutionModifier = 0,
                    SpeedModifier = -5,
                    Prerequisites = new string[0],
                    Exceptions = new string[0],
                    SkillsAmount = 1
                },
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 22007,
                NewDefaultNode("Wi", 22007),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 22008,
                NewDefaultNode("Wi", 22008),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 22009,
                NewDefaultNode("Wi", 22009),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 22010,
                new TalentNode()
                {
                    Position = 22010,
                    Id = "enchanter",
                    Name = "Education",
                    Class = CharacterClass.Enchanter,
                    ClassPoints = 1,
                    UniqueDescription = "Only a knowing one can reach balance between strength and will.",
                    UniqueAction = null,
                    StrengthModifier = 4,
                    WillpowerModifier = 4,
                    ConstitutionModifier = -4,
                    SpeedModifier = -4,
                    Prerequisites = new string[0],
                    Exceptions = new string[0]
                },
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 22012,
                NewDefaultNode("Sw", 22012),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 22014,
                new TalentNode()
                {
                    Position = 22014,
                    Id = "ranger",
                    Name = "Preparation",
                    Class = CharacterClass.Ranger,
                    ClassPoints = 1,
                    UniqueDescription = "Deadly and quick.",
                    UniqueAction = null,
                    StrengthModifier = 4,
                    WillpowerModifier = -4,
                    ConstitutionModifier = -4,
                    SpeedModifier = 4,
                    Prerequisites = new string[0],
                    Exceptions = new string[0]
                },
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 22015,
                NewDefaultNode("Sc", 22015),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 22016,
                NewDefaultNode("Sw", 22016),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 22017,
                NewDefaultNode("Sc", 22017),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 22019,
                NewDefaultNode("Sw", 22019),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 23004,
                NewDefaultNode("Ws", 23004),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 23005,
                NewDefaultNode("Cs", 23005),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 23007,
                NewDefaultNode("Ci", 23007),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 23010,
                NewDefaultNode("Si", 23010),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 23011,
                NewDefaultNode("Wc", 23011),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 23012,
                new TalentNode()
                {
                    Position = 23012,
                    Id = "S",
                    Name = "Strength",
                    Class = null,
                    ClassPoints = 0,
                    UniqueDescription = null,
                    UniqueAction = null,
                    StrengthModifier = 6,
                    WillpowerModifier = -2,
                    ConstitutionModifier = -2,
                    SpeedModifier = -2,
                    Prerequisites = new string[0],
                    Exceptions = new string[0]
                },
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 23013,
                NewDefaultNode("Sc", 23013),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 23014,
                NewDefaultNode("Iw", 23014),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 23016,
                NewDefaultNode("Sw", 23016),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 23019,
                NewDefaultNode("Sw", 23019),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 24005,
                NewDefaultNode("Cs", 24005),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 24006,
                NewDefaultNode("Ci", 24006),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 24007,
                NewDefaultNode("Ci", 24007),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 24008,
                NewDefaultNode("Cs", 24008),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 24011,
                new TalentNode()
                {
                    Position = 24011,
                    Id = "C",
                    Name = "Constitution",
                    Class = null,
                    ClassPoints = 0,
                    UniqueDescription = null,
                    UniqueAction = null,
                    StrengthModifier = -2,
                    WillpowerModifier = -2,
                    ConstitutionModifier = 6,
                    SpeedModifier = -2,
                    Prerequisites = new string[0],
                    Exceptions = new string[0]
                },
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 24013,
                new TalentNode()
                {
                    Position = 24013,
                    Id = "I",
                    Name = "Initiative",
                    Class = null,
                    ClassPoints = 0,
                    UniqueDescription = null,
                    UniqueAction = null,
                    StrengthModifier = -2,
                    WillpowerModifier = -2,
                    ConstitutionModifier = -2,
                    SpeedModifier = 6,
                    Prerequisites = new string[0],
                    Exceptions = new string[0]
                },
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 24016,
                NewDefaultNode("Sw", 24016),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 24017,
                NewDefaultNode("Is", 24017),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 24018,
                NewDefaultNode("Is", 24018),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 24019,
                NewDefaultNode("Sw", 24019),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 25005,
                NewDefaultNode("Cs", 25005),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 25008,
                NewDefaultNode("Cs", 25008),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 25010,
                NewDefaultNode("Wi", 25010),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 25011,
                NewDefaultNode("Cs", 25011),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 25012,
                new TalentNode()
                {
                    Position = 25012,
                    Id = "W",
                    Name = "Willpower",
                    Class = null,
                    ClassPoints = 0,
                    UniqueDescription = null,
                    UniqueAction = null,
                    StrengthModifier = -2,
                    WillpowerModifier = 6,
                    ConstitutionModifier = -2,
                    SpeedModifier = -2,
                    Prerequisites = new string[0],
                    Exceptions = new string[0]
                },
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 25013,
                NewDefaultNode("Is", 25013),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 25014,
                NewDefaultNode("Cw", 25014),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 25017,
                NewDefaultNode("Is", 25017),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 25019,
                NewDefaultNode("Ic", 25019),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 25020,
                NewDefaultNode("Ic", 25020),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 26005,
                NewDefaultNode("Wi", 26005),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 26007,
                NewDefaultNode("Cs", 26007),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 26008,
                NewDefaultNode("Ci", 26008),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 26009,
                NewDefaultNode("Ci", 26009),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 26010,
                new TalentNode()
                {
                    Position = 26010,
                    Id = "bloodletter",
                    Name = "Forbidden rituals",
                    Class = CharacterClass.Bloodletter,
                    ClassPoints = 1,
                    UniqueDescription = "Life is only illusion. Blood is all.",
                    UniqueAction = null,
                    StrengthModifier = -4,
                    WillpowerModifier = 4,
                    ConstitutionModifier = 4,
                    SpeedModifier = -4,
                    Prerequisites = new string[0],
                    Exceptions = new string[0]
                },
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 26012,
                NewDefaultNode("Ws", 26012),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 26014,
                new TalentNode()
                {
                    Position = 26014,
                    Id = "architect",
                    Name = "Alteration",
                    Class = CharacterClass.Architect,
                    ClassPoints = 1,
                    UniqueDescription = "Prepare your soul and whole world will be yours.",
                    UniqueAction = null,
                    StrengthModifier = -4,
                    WillpowerModifier = -4,
                    ConstitutionModifier = 4,
                    SpeedModifier = 4,
                    Prerequisites = new string[0],
                    Exceptions = new string[0]
                },
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 26015,
                NewDefaultNode("Iw", 26015),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 26016,
                NewDefaultNode("Iw", 26016),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 26017,
                NewDefaultNode("Iw", 26017),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 26019,
                new TalentNode()
                {
                    Position = 26019,
                    Id = "mistwalk",
                    Name = "Mistwalk",
                    Class = CharacterClass.Ranger,
                    ClassPoints = 1,
                    UniqueDescription = "Learns the \"Mistwalk\" ability. Teleports within a range of 5. Costs 1 action point. Recharges in 5 turns.",
                    UniqueAction = "LearnMistwalk",
                    StrengthModifier = -3,
                    WillpowerModifier = -5,
                    ConstitutionModifier = 0,
                    SpeedModifier = 0,
                    Prerequisites = new string[0],
                    Exceptions = new string[0],
                    SkillsAmount = 1
                },
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 26020,
                NewDefaultNode("Ic", 26020),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 27005,
                NewDefaultNode("Wi", 27005),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 27006,
                NewDefaultNode("Wi", 27006),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 27007,
                new TalentNode()
                {
                    Position = 27007,
                    Id = "bloodsphere",
                    Name = "Blood sphere",
                    Class = CharacterClass.Bloodletter,
                    ClassPoints = 1,
                    UniqueDescription = "Learns the \"Blood sphere\" ability. Replaces \"Magic missle\" with skill that can be cast every turn, but every time it reduces health of caster.",
                    UniqueAction = "LearnBloodsphere",
                    StrengthModifier = 0,
                    WillpowerModifier = 0,
                    ConstitutionModifier = 0,
                    SpeedModifier = 0,
                    Prerequisites = new string[0],
                    Exceptions = new string[] { "warden" },
                    SkillsAmount = 0
                },
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 27009,
                NewDefaultNode("Ws", 27009),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 27010,
                NewDefaultNode("Ws", 27010),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 27012,
                NewDefaultNode("Ic", 27012),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 27014,
                NewDefaultNode("Cs", 27014),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 27015,
                NewDefaultNode("Cs", 27015),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 27019,
                NewDefaultNode("Iw", 27019),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 27020,
                NewDefaultNode("Iw", 27020),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 28005,
                NewDefaultNode("Ci", 28005),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 28009,
                NewDefaultNode("Ws", 28009),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 28011,
                NewDefaultNode("Ws", 28011),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 28012,
                new TalentNode()
                {
                    Position = 28012,
                    Id = "mistcaller",
                    Name = "Meditation",
                    Class = CharacterClass.Mistcaller,
                    ClassPoints = 1,
                    UniqueDescription = "Stay awhile and listen.",
                    UniqueAction = null,
                    StrengthModifier = -4,
                    WillpowerModifier = 4,
                    ConstitutionModifier = -4,
                    SpeedModifier = 4,
                    Prerequisites = new string[0],
                    Exceptions = new string[0]
                },
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 28013,
                NewDefaultNode("Ic", 28013),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 28015,
                NewDefaultNode("Cs", 28015),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 28016,
                new TalentNode()
                {
                    Position = 28016,
                    Id = "barrier",
                    Name = "Barrier",
                    Class = CharacterClass.Architect,
                    ClassPoints = 1,
                    UniqueDescription = "Learns the \"Barrier\" ability. Creates a barrier within a range of 4. Costs 4 action points. Can be cast every turn.",
                    UniqueAction = "LearnBarrier",
                    StrengthModifier = 0,
                    WillpowerModifier = 0,
                    ConstitutionModifier = -3,
                    SpeedModifier = -3,
                    Prerequisites = new string[0],
                    Exceptions = new string[0],
                    SkillsAmount = 1
                },
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 28019,
                NewDefaultNode("Iw", 28019),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 29005,
                NewDefaultNode("Ci", 29005),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 29009,
                NewDefaultNode("Wi", 29009),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 29011,
                NewDefaultNode("Ws", 29011),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 29013,
                NewDefaultNode("Ic", 29013),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 29016,
                NewDefaultNode("Is", 29016),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 29017,
                NewDefaultNode("Is", 29017),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 29018,
                NewDefaultNode("Cw", 29018),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 29019,
                NewDefaultNode("Cw", 29019),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 30005,
                NewDefaultNode("Cs", 30005),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 30009,
                NewDefaultNode("Wi", 30009),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 30011,
                NewDefaultNode("Ws", 30011),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 30013,
                NewDefaultNode("Ic", 30013),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 30016,
                NewDefaultNode("Is", 30016),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 30019,
                NewDefaultNode("Cs", 30019),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 31005,
                NewDefaultNode("Cs", 31005),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 31006,
                new TalentNode()
                {
                    Position = 31006,
                    Id = "mistpact",
                    Name = "Mist Pact",
                    Class = CharacterClass.Mistcaller,
                    ClassPoints = 1,
                    UniqueDescription = "Learns the \"Mist Pact\" ability. Summons a spawn nearby that attacks enemies. Costs 2 action points. Recharges in 8 turns.",
                    UniqueAction = "LearnMistPact",
                    StrengthModifier = 0,
                    WillpowerModifier = -5,
                    ConstitutionModifier = -5,
                    SpeedModifier = 0,
                    Prerequisites = new string[0],
                    Exceptions = new string[0],
                    SkillsAmount = 1
                },
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 31007,
                NewDefaultNode("Wc", 31007),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 31008,
                NewDefaultNode("Wc", 31008),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 31009,
                NewDefaultNode("Wi", 31009),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 31010,
                NewDefaultNode("Wc", 31010),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 31011,
                new TalentNode()
                {
                    Position = 31011,
                    Id = "wand",
                    Name = "Wand mastery",
                    Class = CharacterClass.Mistcaller,
                    ClassPoints = 1,
                    UniqueDescription = "Replaces attack with the range one that deals willpower-based damage within a range of 4. Can be cast on the targets behind obstactles.",
                    UniqueAction = "LearnWand",
                    StrengthModifier = 0,
                    WillpowerModifier = 0,
                    ConstitutionModifier = 0,
                    SpeedModifier = 0,
                    Prerequisites = new string[0],
                    Exceptions = new string[] { "marksman", "mistweapon" },
                    SkillsAmount = 0
                },
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 31013,
                NewDefaultNode("Cs", 31013),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 31015,
                NewDefaultNode("Iw", 31015),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 31016,
                NewDefaultNode("Iw", 31016),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 31019,
                NewDefaultNode("Ws", 31019),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 32005,
                NewDefaultNode("Cs", 32005),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 32006,
                NewDefaultNode("Is", 32006),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 32010,
                NewDefaultNode("Ws", 32010),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 32011,
                NewDefaultNode("Ws", 32011),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 32013,
                NewDefaultNode("Cs", 32013),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 32014,
                NewDefaultNode("Cs", 32014),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 32015,
                NewDefaultNode("Iw", 32015),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 32018,
                NewDefaultNode("Ws", 32018),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 32019,
                NewDefaultNode("Ws", 32019),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 33006,
                NewDefaultNode("Is", 33006),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 33007,
                NewDefaultNode("Is", 33007),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 33010,
                NewDefaultNode("Ws", 33010),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 33014,
                NewDefaultNode("Iw", 33014),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 33017,
                NewDefaultNode("Is", 33017),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 33018,
                NewDefaultNode("Is", 33018),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 34007,
                NewDefaultNode("Wc", 34007),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 34008,
                NewDefaultNode("Wc", 34008),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 34009,
                NewDefaultNode("Wc", 34009),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 34010,
                NewDefaultNode("Ws", 34010),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 34011,
                NewDefaultNode("Is", 34011),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 34012,
                NewDefaultNode("Is", 34012),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 34013,
                new TalentNode()
                {
                    Position = 34013,
                    Id = "powerplace",
                    Name = "Place of power",
                    Class = CharacterClass.Architect,
                    ClassPoints = 1,
                    UniqueDescription = "Learns the \"Place of power\" ability. Transforms a cell within a range of 4 to a place of power that increases strength and willpower by 200%. Costs 4 action points. Recharges in 6 turns.",
                    UniqueAction = "LearnPowerplace",
                    StrengthModifier = 0,
                    WillpowerModifier = -5,
                    ConstitutionModifier = -5,
                    SpeedModifier = 0,
                    Exceptions = new string[0],
                    Prerequisites = new string[0],
                    SkillsAmount = 1
                },
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 34014,
                NewDefaultNode("Iw", 34014),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 34015,
                NewDefaultNode("Ws", 34015),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 34016,
                NewDefaultNode("Ws", 34016),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 34017,
                NewDefaultNode("Is", 34017),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 35012,
                NewDefaultNode("Is", 35012),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 35013,
                NewDefaultNode("Iw", 35013),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 35014,
                NewDefaultNode("Iw", 35014),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 36012,
                NewDefaultNode("Wc", 36012),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 37011,
                NewDefaultNode("Is", 37011),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 37012,
                NewDefaultNode("Wc", 37012),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 38012,
                NewDefaultNode("Wc", 38012),
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 38013,
                NewDefaultNode("Ic", 38013),
                true);
            context.ApplyChangesAsync().Wait();
        }
    }
}