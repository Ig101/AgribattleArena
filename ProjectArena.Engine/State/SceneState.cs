using System.Collections.Generic;
using ProjectArena.Infrastructure.Enums;
using ProjectArena.Infrastructure.Models.Battle;

namespace ProjectArena.Engine.State
{
    public class SceneState
    {
        public string Id { get; set; }

        public double TimeLine { get; set; }

        public int IdCounterPosition { get; set; }

        public StartTurnInfoDto TurnInfo { get; set; }

        public IEnumerable<ActorSynchronizationDto> Actors { get; set; }

        public IEnumerable<PlayerState> Players { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public Biom Biom { get; set; }
    }
}