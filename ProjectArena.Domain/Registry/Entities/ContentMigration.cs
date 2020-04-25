using System;
using MongoDB.Bson.Serialization.Attributes;

namespace ProjectArena.Domain.Registry.Entities
{
    public class ContentMigration
    {
        [BsonId]
        public string Id { get; set; }
    }
}