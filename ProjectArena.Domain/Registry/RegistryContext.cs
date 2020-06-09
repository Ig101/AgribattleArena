using System;
using System.Linq;
using System.Reflection;
using ProjectArena.Domain.Registry.Entities;
using ProjectArena.Infrastructure.Mongo;

namespace ProjectArena.Domain.Registry
{
    public class RegistryContext : BaseMongoContext
    {
        public IRepository<ContentMigration> Migrations { get; set; }

        public IRepository<TalentNode> TalentMap { get; set; }

        public RegistryContext(IMongoConnection connection)
            : base(connection)
        {
            Migrations = InitializeRepository<ContentMigration>();
            TalentMap = InitializeRepository<TalentNode>();
        }

        public void LoadMigrations()
        {
            var types = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(x => x.GetInterfaces().FirstOrDefault(i => i == typeof(IContentMigration)) != null)
                .OrderBy(x => x.Name)
                .ToList();
            foreach (var type in types)
            {
                if (Migrations.GetOneAsync(x => x.Id == type.Name).Result == null)
                {
                    var migration = (IContentMigration)Activator.CreateInstance(type);
                    migration.Up(this);
                    Migrations.InsertOneAtomicallyAsync(new ContentMigration()
                    {
                        Id = type.Name
                    }).Wait();
                }
            }
        }
    }
}