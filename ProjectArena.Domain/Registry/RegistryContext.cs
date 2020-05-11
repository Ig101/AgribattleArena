using System;
using System.Linq;
using System.Reflection;
using ProjectArena.Domain.Mongo;
using ProjectArena.Domain.Registry.Entities;

namespace ProjectArena.Domain.Registry
{
    public class RegistryContext : BaseMongoContext
    {
        public IRepository<ContentMigration> Migrations { get; set; }

        public IRepository<ActorNative> ActorNatives { get; set; }

        public IRepository<BuffNative> BuffNatives { get; set; }

        public IRepository<DecorationNative> DecorationNatives { get; set; }

        public IRepository<EffectNative> EffectNatives { get; set; }

        public IRepository<RoleModelNative> RoleModelNatives { get; set; }

        public IRepository<SkillNative> SkillNatives { get; set; }

        public IRepository<TileNative> TileNatives { get; set; }

        public RegistryContext(IMongoConnection connection)
            : base(connection)
        {
            Migrations = InitializeRepository<ContentMigration>();
            ActorNatives = InitializeRepository<ActorNative>();
            BuffNatives = InitializeRepository<BuffNative>();
            DecorationNatives = InitializeRepository<DecorationNative>();
            EffectNatives = InitializeRepository<EffectNative>();
            RoleModelNatives = InitializeRepository<RoleModelNative>();
            SkillNatives = InitializeRepository<SkillNative>();
            TileNatives = InitializeRepository<TileNative>();
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