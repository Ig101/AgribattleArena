using System;
using System.Collections.Generic;
using ProjectArena.Infrastructure.Models.Battle;

namespace ProjectArena.Engine.Models
{
    public class OutcomingMessage
    {
        public IEnumerable<Guid> Users { get; set; }

        public SynchronizationMessageDto Message { get; set; }
    }
}