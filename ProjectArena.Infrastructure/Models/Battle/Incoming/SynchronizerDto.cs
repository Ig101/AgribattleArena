using System.Collections.Generic;

namespace ProjectArena.Infrastructure.Models.Battle.Incoming
{
    public class SynchronizerDto
    {
        public int IdCounterPosition { get; set; }

        public IEnumerable<ActorSynchronizationDto> Actors { get; set; }

        public IEnumerable<ActorReferenceDto> RemovedActors { get; set; }
    }
}