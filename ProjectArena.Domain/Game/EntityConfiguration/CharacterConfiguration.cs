using System.Threading.Tasks;
using MongoDB.Driver;
using ProjectArena.Domain.Game.Entities;
using ProjectArena.Infrastructure.Mongo;

namespace ProjectArena.Domain.Game.EntityConfiguration
{
    public class CharacterConfiguration : IEntityConfiguration<Character>
    {
        public async Task ConfigureAsync(IMongoCollection<Character> collection)
        {
            await collection.Indexes.CreateManyAsync(new[]
            {
                new CreateIndexModel<Character>(Builders<Character>.IndexKeys.Ascending(x => x.RosterUserId)),
            });
        }
    }
}