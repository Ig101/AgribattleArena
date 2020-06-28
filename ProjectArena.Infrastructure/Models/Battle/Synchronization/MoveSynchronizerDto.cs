using System;
using System.Collections.Generic;

namespace ProjectArena.Infrastructure.Models.Battle.Synchronization
{
    public class MoveSynchronizerDto
    {
        public Guid Id { get; set; }

        public IEnumerable<MoveInfoDto> Moves { get; set; }
    }
}