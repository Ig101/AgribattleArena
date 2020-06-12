using System;
using System.Collections.Generic;

namespace ProjectArena.Infrastructure.Models.Battle.Synchronization
{
    public class SynchronizerDto
    {
        public Guid Id { get; set; }

        public int Version { get; set; }

        public float RoundsPassed { get; set; }

        public int? ActorId { get; set; }

        public int? SkillActionId { get; set; }

        public int? TargetX { get; set; }

        public int? TargetY { get; set; }

        public float TurnTime { get; set; }

        public int? TempActor { get; set; }

        public int? TempDecoration { get; set; }

        public RewardDto Reward { get; set; }

        public IEnumerable<PlayerDto> Players { get; set; }

        public IEnumerable<ActorDto> ChangedActors { get; set; }

        public IEnumerable<ActiveDecorationDto> ChangedDecorations { get; set; }

        public IEnumerable<SpecEffectDto> ChangedEffects { get; set; }

        public IEnumerable<int> DeletedActors { get; set; }

        public IEnumerable<int> DeletedDecorations { get; set; }

        public IEnumerable<int> DeletedEffects { get; set; }

        public IEnumerable<TileDto> ChangedTiles { get; set; }

        public int TilesetWidth { get; set; }

        public int TilesetHeight { get; set; }
    }
}
