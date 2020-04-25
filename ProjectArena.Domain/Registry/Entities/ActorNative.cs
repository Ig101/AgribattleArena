using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using ProjectArena.Domain.Registry.EntityModels;

namespace ProjectArena.Domain.Registry.Entities
{
    public class ActorNative
    {
        [BsonId]
        public string Id { get; set; }

        [BsonElement("t")]
        public IEnumerable<string> Tags { get; set; }

        [BsonElement("z")]
        public float DefaultZ { get; set; }

        [BsonElement("a")]
        public IEnumerable<TagSynergy> Armor { get; set; }
    }
}