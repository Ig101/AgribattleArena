using System.Threading.Tasks;
using MongoDB.Driver;
using ProjectArena.Domain.Identity.Entities;
using ProjectArena.Domain.Mongo;

namespace ProjectArena.Domain.Identity.EntityConfiguration
{
    public class RoleConfiguration : IEntityConfiguration<Role>
    {
        public async Task ConfigureAsync(IMongoCollection<Role> collection)
        {
            await collection.Indexes.CreateManyAsync(new[]
            {
                new CreateIndexModel<Role>(Builders<Role>.IndexKeys.Ascending(x => x.NormalizedName)),
            });
        }
    }
}
