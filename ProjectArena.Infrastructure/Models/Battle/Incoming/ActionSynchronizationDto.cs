namespace ProjectArena.Infrastructure.Models.Battle.Incoming
{
    public class ActionSynchronizationDto
    {
        public int Version { get; set; }

        public string Code { get; set; }

        public SynchronizerDto Synchronizer { get; set; }
    }
}