using MongoDB.Driver;

namespace ProjectArena.Domain.Mongo
{
    public interface IMongoConnection
    {
        bool UseTransactions { get; }

        IMongoCollection<T> GetCollection<T>();

        IClientSessionHandle StartSession();
    }
}