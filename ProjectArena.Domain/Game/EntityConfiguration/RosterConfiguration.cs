using System.Threading.Tasks;
using MongoDB.Driver;
using ProjectArena.Domain.Game.Entities;
using ProjectArena.Domain.Mongo;

namespace ProjectArena.Domain.Game.EntityConfiguration
{
    public class RosterConfiguration : IEntityConfiguration<Roster>
    {
        public async Task ConfigureAsync(IMongoCollection<Roster> collection)
        {
            await collection.Indexes.CreateManyAsync(new[]
            {
                new CreateIndexModel<Roster>(Builders<Roster>.IndexKeys.Ascending(x => x.UserId)),
            });
        }
    }
}