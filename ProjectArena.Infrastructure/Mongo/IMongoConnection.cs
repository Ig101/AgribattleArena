using MongoDB.Driver;

namespace ProjectArena.Infrastructure.Mongo
{
    public interface IMongoConnection
    {
        bool UseTransactions { get; }

        IMongoCollection<T> GetCollection<T>();

        IClientSessionHandle StartSession();
    }
}