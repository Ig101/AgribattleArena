using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace ProjectArena.Domain.Registry.Entities
{
    public class EffectNative
    {
        [BsonId]
        public string Id { get; set; }

        [BsonElement("t")]
        public IEnumerable<string> Tags { get; set; }

        [BsonElement("z")]
        public float DefaultZ { get; set; }

        [BsonElement("dd")]
        public float? DefaultDuration { get; set; }

        [BsonElement("m")]
        public float DefaultMod { get; set; }

        [BsonElement("a")]
        public IEnumerable<string> Actions { get; set; }

        [BsonElement("d")]
        public IEnumerable<string> OnDeathActions { get; set; }
    }
}