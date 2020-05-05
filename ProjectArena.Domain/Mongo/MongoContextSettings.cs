namespace ProjectArena.Domain.Mongo
{
    public class MongoContextSettings<T> : IMongoContextSettings
    {
        public string NamespaceName { get; set; }
    }
}