using System;
using System.Collections.Generic;
using System.Linq;
using ProjectArena.Domain.Registry.Entities;
using ProjectArena.Infrastructure.Enums;

namespace ProjectArena.Domain.Registry.Migrations
{
    public class M0001_AddUniqueTalents : IContentMigration
    {
        public void Up(RegistryContext context)
        {
            // Stats
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
                    Exceptions = new string[0]
                },
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
                    Exceptions = new string[0]
                },
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
                    Exceptions = new string[0]
                },
                true);

            // Classes
            context.TalentMap.ReplaceOne(
                x => x.Position == 23011,
                new TalentNode()
                {
                    Position = 23011,
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
                    Exceptions = new string[0]
                },
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 25011,
                new TalentNode()
                {
                    Position = 25011,
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
                    Exceptions = new string[0]
                },
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 23013,
                new TalentNode()
                {
                    Position = 23013,
                    Id = "ranger",
                    Name = "Fencing",
                    Class = CharacterClass.Ranger,
                    ClassPoints = 1,
                    UniqueDescription = "Deadly and quick.",
                    UniqueAction = null,
                    StrengthModifier = 4,
                    WillpowerModifier = -4,
                    ConstitutionModifier = -4,
                    SpeedModifier = 4,
                    Exceptions = new string[0]
                },
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 25013,
                new TalentNode()
                {
                    Position = 25013,
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
                    Exceptions = new string[0]
                },
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 22012,
                new TalentNode()
                {
                    Position = 22012,
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
                    Exceptions = new string[0]
                },
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 26012,
                new TalentNode()
                {
                    Position = 26012,
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
                    Exceptions = new string[0]
                },
                true);

            // Skills
            context.TalentMap.ReplaceOne(
                x => x.Position == 17012,
                new TalentNode()
                {
                    Position = 17012,
                    Id = "leap",
                    Name = "Leap",
                    Class = CharacterClass.Fighter,
                    ClassPoints = 1,
                    UniqueDescription = "Learns \"Leap\" ability. Leaps to the position within a range of 3 and deal small damage to all surrounding objects. Costs 3 action points. Recharges in 3 turns.",
                    UniqueAction = "LearnLeap",
                    StrengthModifier = 0,
                    WillpowerModifier = -3,
                    ConstitutionModifier = 0,
                    SpeedModifier = -5,
                    Exceptions = new string[0],
                    SkillsAmount = 1
                },
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 18014,
                new TalentNode()
                {
                    Position = 18014,
                    Id = "warden",
                    Name = "Warden",
                    Class = CharacterClass.Fighter,
                    ClassPoints = 1,
                    UniqueDescription = "Learns \"Warden\" ability. Replaces \"Magic missle\" with weapon throw that deals strength-based damage within a range of 3.",
                    UniqueAction = "LearnWarden",
                    StrengthModifier = 0,
                    WillpowerModifier = 0,
                    ConstitutionModifier = 0,
                    SpeedModifier = 0,
                    Exceptions = new string[] { "bloodbowl" },
                    SkillsAmount = 0
                },
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 21016,
                new TalentNode()
                {
                    Position = 21016,
                    Id = "marksman",
                    Name = "Marksman",
                    Class = CharacterClass.Ranger,
                    ClassPoints = 1,
                    UniqueDescription = "Replaces attack with the range one that deals slightly less damage within a range of 4.",
                    UniqueAction = "LearnMarksman",
                    StrengthModifier = 0,
                    WillpowerModifier = 0,
                    ConstitutionModifier = 0,
                    SpeedModifier = 0,
                    Exceptions = new string[] { "wand" },
                    SkillsAmount = 0
                },
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 23017,
                new TalentNode()
                {
                    Position = 23017,
                    Id = "shadowstep",
                    Name = "Shadowstep",
                    Class = CharacterClass.Ranger,
                    ClassPoints = 1,
                    UniqueDescription = "Learns \"Shadowstep\" ability. Teleports within a range of 4. Costs 1 action point. Recharges in 3 turns.",
                    UniqueAction = "LearnShadowstep",
                    StrengthModifier = 0,
                    WillpowerModifier = -3,
                    ConstitutionModifier = -5,
                    SpeedModifier = 0,
                    Exceptions = new string[0],
                    SkillsAmount = 1
                },
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 27016,
                new TalentNode()
                {
                    Position = 27016,
                    Id = "powerplace",
                    Name = "Place of power",
                    Class = CharacterClass.Architect,
                    ClassPoints = 1,
                    UniqueDescription = "Learns \"Place of power\" ability. Transforms a cell within a range of 4 to a place of power that increases all outcoming magic damage for 100%. Costs 4 action points. Recharges in 3 turns.",
                    UniqueAction = "LearnPowerplace",
                    StrengthModifier = -5,
                    WillpowerModifier = -5,
                    ConstitutionModifier = 0,
                    SpeedModifier = 0,
                    Exceptions = new string[0],
                    SkillsAmount = 1
                },
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 29015,
                new TalentNode()
                {
                    Position = 29015,
                    Id = "barrier",
                    Name = "Barrier",
                    Class = CharacterClass.Architect,
                    ClassPoints = 1,
                    UniqueDescription = "Learns \"Barrier\" ability. Creates a barrier within a range of 4. Costs 4 action points. Can be cast every turn.",
                    UniqueAction = "LearnBarrier",
                    StrengthModifier = -3,
                    WillpowerModifier = -3,
                    ConstitutionModifier = 0,
                    SpeedModifier = 0,
                    Exceptions = new string[0],
                    SkillsAmount = 1
                },
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 31012,
                new TalentNode()
                {
                    Position = 31012,
                    Id = "wand",
                    Name = "Phoenix wand",
                    Class = CharacterClass.Mistcaller,
                    ClassPoints = 1,
                    UniqueDescription = "Replaces attack with the range one that deals willpower-based damage within a range of 3.",
                    UniqueAction = "LearnWand",
                    StrengthModifier = 0,
                    WillpowerModifier = 0,
                    ConstitutionModifier = 0,
                    SpeedModifier = 0,
                    Exceptions = new string[] { "marksman", "mistweapon" },
                    SkillsAmount = 0
                },
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 30010,
                new TalentNode()
                {
                    Position = 30010,
                    Id = "mistpact",
                    Name = "Mist Pact",
                    Class = CharacterClass.Mistcaller,
                    ClassPoints = 1,
                    UniqueDescription = "Learns \"Mist Pact\" ability. Summons a spawn nearby that attacks enemies. Costs 2 action points. Recharges in 8 turns.",
                    UniqueAction = "LearnMistPact",
                    StrengthModifier = 0,
                    WillpowerModifier = -5,
                    ConstitutionModifier = -5,
                    SpeedModifier = 0,
                    Exceptions = new string[0],
                    SkillsAmount = 1
                },
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 27008,
                new TalentNode()
                {
                    Position = 27008,
                    Id = "bloodbowl",
                    Name = "Blood bowl",
                    Class = CharacterClass.Bloodletter,
                    ClassPoints = 1,
                    UniqueDescription = "Learns \"Blood bowl\" ability. Replaces \"Magic missle\" with skill that can be cast every turn, but every time it reduces health of caster.",
                    UniqueAction = "LearnBloodBowl",
                    StrengthModifier = 0,
                    WillpowerModifier = 0,
                    ConstitutionModifier = 0,
                    SpeedModifier = 0,
                    Exceptions = new string[] { "warden" },
                    SkillsAmount = 0
                },
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 25007,
                new TalentNode()
                {
                    Position = 25007,
                    Id = "offspring",
                    Name = "Offspring",
                    Class = CharacterClass.Bloodletter,
                    ClassPoints = 1,
                    UniqueDescription = "Learns \"Offspring\" ability. Summons a spawn with 20% of caster's health nearby and reduces health of caster by that amount. Can be sacrificed to heal organic object nearby by its amount of health. Costs 2 action points. Recharges in 8 turns.",
                    UniqueAction = "LearnOffspring",
                    StrengthModifier = 0,
                    WillpowerModifier = -5,
                    ConstitutionModifier = 0,
                    SpeedModifier = -5,
                    Exceptions = new string[0],
                    SkillsAmount = 1
                },
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 21008,
                new TalentNode()
                {
                    Position = 21008,
                    Id = "armaments",
                    Name = "Armaments",
                    Class = CharacterClass.Enchanter,
                    ClassPoints = 1,
                    UniqueDescription = "Learns \"Armaments\" ability. Increases organic target's damage using magic damage modifier up to 50% for 3 turns. Costs 6 action points. Recharges in 4 turns.",
                    UniqueAction = "LearnArmaments",
                    StrengthModifier = -5,
                    WillpowerModifier = 0,
                    ConstitutionModifier = 0,
                    SpeedModifier = -5,
                    Exceptions = new string[0],
                    SkillsAmount = 1
                },
                true);
            context.TalentMap.ReplaceOne(
                x => x.Position == 19009,
                new TalentNode()
                {
                    Position = 19009,
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
                    Exceptions = new string[] { "wand" },
                    SkillsAmount = 0
                },
                true);
            context.ApplyChangesAsync().Wait();
        }
    }
}