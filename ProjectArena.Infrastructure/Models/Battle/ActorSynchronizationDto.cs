using System.Collections;
using System.Collections.Generic;
using ProjectArena.Infrastructure.Models.Game;

namespace ProjectArena.Infrastructure.Models.Battle
{
    public class ActorSynchronizationDto
    {
        public ActorReferenceDto Reference { get; set; }

        public int Position { get; set; }

        public string Name { get; set; }

        public string Char { get; set; }

        public ColorDto Color { get; set; }

        public bool Left { get; set; }

        public string OwnerId { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public int ParentId { get; set; }

        public float Durability { get; set; }

        public float MaxDurability { get; set; }

        public float TurnCost { get; set; }

        public float InitiativePosition { get; set; }

        public int Height { get; set; }

        public int Volume { get; set; }

        public int FreeVolume { get; set; }

        public IEnumerable<ReactionSynchronizationDto> PreparationReactions { get; set; }

        public IEnumerable<ReactionSynchronizationDto> ActiveReactions { get; set; }

        public IEnumerable<ReactionSynchronizationDto> ClearReactions { get; set; }

        public IEnumerable<ActionSynchronizationDto> Actions { get; set; }

        public IList<ActorSynchronizationDto> Actors { get; set; }

        public IEnumerable<BuffSynchronizationDto> Buffs { get; set; }
    }
}