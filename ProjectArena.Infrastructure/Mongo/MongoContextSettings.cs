namespace ProjectArena.Infrastructure.Mongo
{
    public class MongoContextSettings<T> : IMongoContextSettings
    {
        public string NamespaceName { get; set; }
    }
}