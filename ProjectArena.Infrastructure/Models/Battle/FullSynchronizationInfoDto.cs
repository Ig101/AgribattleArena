using System.Collections.Generic;
using ProjectArena.Infrastructure.Enums;

namespace ProjectArena.Infrastructure.Models.Battle
{
    public class FullSynchronizationInfoDto
    {
        public string Id { get; set; }

        public double TimeLine { get; set; }

        public int IdCounterPosition { get; set; }

        public string CurrentPlayerId { get; set; }

        public IEnumerable<ActorSynchronizationDto> Actors { get; set; }

        public IEnumerable<PlayerSynchronizationDto> Players { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public Biom Biom { get; set; }

        public IEnumerable<SynchronizationMessageDto> WaitingActions { get; set; }
    }
}