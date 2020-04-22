using System;

namespace ProjectArena.Infrastructure.Models.Game
{
    public class GameStatusDto
    {
        public Guid GameId { get; set; }

        public string PlayerName { get; set; }
    }
}