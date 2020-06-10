using System;
using System.Collections.Generic;
using System.Linq;
using ProjectArena.Engine.ForExternalUse.Synchronization.ObjectInterfaces;
using ProjectArena.Engine.Helpers;

namespace ProjectArena.Engine.Synchronizers.SynchronizationObjects
{
    internal class Actor : IActor, ForExternalUse.Generation.ObjectInterfaces.IActor
    {
        public string AttackingSkillName { get; }

        public IEnumerable<string> SkillNames { get; }

        public IEnumerable<string> StartBuffs { get; }

        public int Id { get; }

        public Guid? ExternalId { get; }

        public string NativeId { get; }

        public ISkill AttackingSkill { get; }

        public int Strength { get; }

        public int Willpower { get; }

        public int Constitution { get; }

        public int Speed { get; }

        public string OwnerId { get; }

        public int? Team { get; }

        public List<ISkill> Skills { get; }

        public int ActionPointsIncome { get; }

        public List<IBuff> Buffs { get; }

        public float InitiativePosition { get; }

        public float Health { get; }

        public bool IsAlive { get; }

        public int X { get; }

        public int Y { get; }

        public float Z { get; }

        public int MaxHealth { get; }

        public int ActionPoints { get; }

        public float SkillPower { get; }

        public float AttackPower { get; }

        public float Initiative { get; }

        public List<TagSynergy> Armor { get; }

        public List<TagSynergy> AttackModifiers { get; }

        public bool CanMove { get; }

        public bool CanAct { get; }

        public bool HealthRevealed { get; }

        public Actor(Objects.Actor actor)
        {
            this.Id = actor.Id;
            this.ExternalId = actor.ExternalId;
            this.NativeId = actor.Native.Id;
            this.AttackingSkill = new Skill(actor.AttackingSkill);
            this.ActionPointsIncome = actor.ActionPointsIncome;
            this.Strength = actor.Strength;
            this.Willpower = actor.Willpower;
            this.Constitution = actor.Constitution;
            this.Speed = actor.Speed;
            this.OwnerId = actor.Owner?.Id;
            this.Team = actor.Owner?.Team;
            this.Skills = actor.Skills.Select(x => (ISkill)new Skill(x)).ToList();
            this.Buffs = actor.Buffs.Select(x => (IBuff)new Buff(x)).ToList();
            this.InitiativePosition = actor.InitiativePosition;
            this.Health = actor.DamageModel.Health;
            this.IsAlive = actor.IsAlive;
            this.X = actor.X;
            this.Y = actor.Y;
            this.Z = actor.Z;
            this.MaxHealth = actor.MaxHealth;
            this.ActionPoints = actor.ActionPoints;
            this.SkillPower = actor.SkillPower;
            this.AttackPower = actor.AttackPower;
            this.Initiative = actor.Initiative;
            this.Armor = new List<TagSynergy>();
            this.Armor.AddRange(actor.Armor);
            this.AttackModifiers = new List<TagSynergy>();
            this.AttackModifiers.AddRange(actor.AttackModifiers);
            this.CanMove = actor.BuffManager.CanMove;
            this.CanAct = actor.BuffManager.CanAct;
            this.HealthRevealed = actor.HealthRevealed;
        }

        public Actor(
            Guid? externalId,
            string nativeId,
            string attackingSkillName,
            int strength,
            int willpower,
            int constitution,
            int speed,
            IEnumerable<string> skillNames,
            int actionPointsIncome,
            IEnumerable<string> startBuffs)
        {
            this.StartBuffs = startBuffs;
            this.ExternalId = externalId;
            this.NativeId = nativeId;
            this.AttackingSkillName = attackingSkillName;
            this.Strength = strength;
            this.Willpower = willpower;
            this.Constitution = constitution;
            this.Speed = speed;
            this.SkillNames = skillNames;
            this.ActionPointsIncome = actionPointsIncome;
        }
    }
}
