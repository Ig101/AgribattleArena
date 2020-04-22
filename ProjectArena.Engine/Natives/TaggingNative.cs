namespace AgribattleArena.Engine.Natives
{
    public class TaggingNative
    {
        public string Id { get; }

        public string[] Tags { get; }

        public TaggingNative(string id, string[] tags)
        {
            this.Id = id;
            this.Tags = tags;
        }
    }
}
