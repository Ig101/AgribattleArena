using ProjectArena.Infrastructure.Models.Battle.Incoming;

namespace ProjectArena.Engine.Models
{
    public class IncomingAction : ActionDto
    {
        public string UserId { get; set; }
    }
}