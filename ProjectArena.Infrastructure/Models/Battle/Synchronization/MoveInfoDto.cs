using System;
using System.Collections.Generic;

namespace ProjectArena.Infrastructure.Models.Battle.Synchronization
{
    public class MoveInfoDto
    {
        public int ActorId { get; set; }

        public int TargetX { get; set; }

        public int TargetY { get; set; }
    }
}
