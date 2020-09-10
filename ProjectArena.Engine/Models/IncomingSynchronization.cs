using ProjectArena.Infrastructure.Models.Battle.Incoming;

namespace ProjectArena.Engine.Models
{
    public class IncomingSynchronization : SynchronizationDto
    {
        public string UserId { get; set; }
    }
}