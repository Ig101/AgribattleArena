using System.Collections.Generic;

namespace ProjectArena.Infrastructure.Models.Battle.Synchronization
{
    public class ActorDto
    {
        public int Id { get; set; }

        public long? ExternalId { get; set; }

        public string NativeId { get; set; }

        public SkillDto AttackingSkill { get; set; }

        public int Strength { get; set; }

        public int Willpower { get; set; }

        public int Constitution { get; set; }

        public int Speed { get; set; }

        public IEnumerable<SkillDto> Skills { get; set; }

        public int ActionPointsIncome { get; set; }

        public IEnumerable<BuffDto> Buffs { get; set; }

        public float InitiativePosition { get; set; }

        public float Health { get; set; }

        public string OwnerId { get; set; }

        public bool IsAlive { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public float Z { get; set; }

        public int MaxHealth { get; set; }

        public int ActionPoints { get; set; }

        public float SkillPower { get; set; }

        public float AttackPower { get; set; }

        public float Initiative { get; set; }

        public IEnumerable<TagSynergyDto> Armor { get; set; }

        public IEnumerable<TagSynergyDto> AttackModifiers { get; set; }

        public bool CanMove { get; set; }

        public bool CanAct { get; set; }
    }
}
