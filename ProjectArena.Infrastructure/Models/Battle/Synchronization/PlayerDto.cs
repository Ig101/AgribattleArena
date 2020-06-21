using System.Collections.Generic;
using ProjectArena.Infrastructure.Enums;

namespace ProjectArena.Infrastructure.Models.Battle.Synchronization
{
    public class PlayerDto
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public int? Team { get; set; }

        public IEnumerable<int> KeyActorsSync { get; set; }

        public int TurnsSkipped { get; set; }

        public PlayerStatus Status { get; set; }
    }
}
