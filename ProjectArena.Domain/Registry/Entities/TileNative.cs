using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace ProjectArena.Domain.Registry.Entities
{
    public class TileNative
    {
        [BsonId]
        public string Id { get; set; }

        [BsonElement("t")]
        public IEnumerable<string> Tags { get; set; }

        [BsonElement("f")]
        public bool Flat { get; set; }

        [BsonElement("h")]
        public int DefaultHeight { get; set; }

        [BsonElement("u")]
        public bool Unbearable { get; set; }

        [BsonElement("m")]
        public float DefaultMod { get; set; }

        [BsonElement("a")]
        public IEnumerable<string> Actions { get; set; }

        [BsonElement("s")]
        public IEnumerable<string> OnStepActions { get; set; }
    }
}