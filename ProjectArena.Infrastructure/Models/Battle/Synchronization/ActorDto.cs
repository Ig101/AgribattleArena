using System;
using System.Collections.Generic;

namespace ProjectArena.Infrastructure.Models.Battle.Synchronization
{
    public class ActorDto
    {
        public int Id { get; set; }

        public Guid? ExternalId { get; set; }

        public string NativeId { get; set; }

        public string Visualization { get; set; }

        public SkillDto AttackingSkill { get; set; }

        public IEnumerable<SkillDto> Skills { get; set; }

        public IEnumerable<BuffDto> Buffs { get; set; }

        public float? Health { get; set; }

        public string OwnerId { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public float Z { get; set; }

        public int? MaxHealth { get; set; }

        public float Initiative { get; set; }

        public bool CanMove { get; set; }

        public bool CanAct { get; set; }
    }
}
