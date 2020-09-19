using System.Collections.Generic;
using ProjectArena.Infrastructure.Models.Battle;

namespace ProjectArena.Engine.Models
{
    public class OutcomingMessage
    {
        public IEnumerable<string> Users { get; set; }

        public SynchronizationMessageDto Message { get; set; }
    }
}