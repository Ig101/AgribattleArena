namespace ProjectArena.Infrastructure.Models.Battle.Incoming
{
    public class ActionDto
    {
        public string SceneId { get; set; }

        public int Version { get; set; }

        public string Code { get; set; }

        public string NewCode { get; set; }

        public ActionInfoDto Action { get; set; }
    }
}