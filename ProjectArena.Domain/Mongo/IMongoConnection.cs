using MongoDB.Driver;

namespace ProjectArena.Domain.Mongo
{
    public interface IMongoConnection
    {
        IMongoCollection<T> GetCollection<T>();

        IClientSessionHandle StartSession();
    }
}