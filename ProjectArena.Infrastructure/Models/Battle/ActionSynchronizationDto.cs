namespace ProjectArena.Infrastructure.Models.Battle
{
    public class ActionSynchronizationDto
    {
        public string Id { get; set; }

        public bool IsAutomatic { get; set; }

        public float RemainedTime { get; set; }
    }
}