using ProjectArena.Infrastructure.Enums;

namespace ProjectArena.Infrastructure.Models.Queue
{
    public class UserInQueueDto
    {
        public GameMode Mode { get; set; }

        public int Time { get; set; }
    }
}