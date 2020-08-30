namespace ProjectArena.Infrastructure.Models.Battle.Incoming
{
    public class ActionDto
    {
        public int Version { get; set; }

        public string Code { get; set; }

        public string NewCode { get; set; }

        public ActionInfoDto Action { get; set; }
    }
}