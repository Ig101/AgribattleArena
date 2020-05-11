using System;
using System.Collections.Generic;
using ProjectArena.Infrastructure.Enums;
using ProjectArena.Infrastructure.Models.Game;
using ProjectArena.Infrastructure.Models.User;

namespace ProjectArena.Infrastructure.Models.Queue
{
    public class UserToEnqueueDto
    {
        public string UserId { get; set; }

        public GameMode Mode { get; set; }
    }
}