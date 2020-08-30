using System.Collections;
using System.Collections.Generic;

namespace ProjectArena.Infrastructure.Models.Battle
{
    public class BuffSynchronizationDto
    {
        public string Id { get; set; }

        public float Duration { get; set; }

        public int MaxStacks { get; set; }

        public int Counter { get; set; }

        public float ChangedDurability { get; set; }

        public float ChangedSpeed { get; set; }

        public IEnumerable<ActionSynchronizationDto> Actions { get; set; }

        public IEnumerable<ReactionSynchronizationDto> AddedPreparationReactions { get; set; }

        public IEnumerable<ReactionSynchronizationDto> AddedActiveReactions { get; set; }

        public IEnumerable<ReactionSynchronizationDto> AddedClearReactions { get; set; }
    }
}