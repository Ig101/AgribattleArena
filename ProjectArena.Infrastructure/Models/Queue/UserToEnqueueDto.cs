using System;
using ProjectArena.Infrastructure.Enums;

namespace ProjectArena.Infrastructure.Models.Queue
{
    public class UserToEnqueueDto
    {
        public string UserId { get; set; }

        public GameMode Mode { get; set; }
    }
}