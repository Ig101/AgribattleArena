using System.Threading.Tasks;
using MongoDB.Driver;

namespace ProjectArena.Infrastructure.Mongo
{
    public interface IEntityConfiguration<Ttype>
    {
        Task ConfigureAsync(IMongoCollection<Ttype> collection);
    }
}