using System.Collections.Generic;

namespace ProjectArena.Infrastructure.Models.Game
{
    public class GameStateDto
    {
        public IEnumerable<SceneSegmentDto> SceneSegments { get; set; }

        public IEnumerable<UnsettledActorDto> UnsettledActors { get; set; }
    }
}