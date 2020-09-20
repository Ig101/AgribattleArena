namespace ProjectArena.Infrastructure.Models.Battle.Incoming
{
    public class SynchronizationDto
    {
        public string SceneId { get; set; }

        public int Version { get; set; }

        public string Code { get; set; }

        public SynchronizerDto Synchronizer { get; set; }
    }
}