namespace ProjectArena.Domain
{
    public class MongoContextSettings<T> : IMongoContextSettings
    {
        public string DatabaseName { get; set; }
    }
}