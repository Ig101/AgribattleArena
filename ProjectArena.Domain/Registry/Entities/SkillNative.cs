using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace ProjectArena.Domain.Registry.Entities
{
    public class SkillNative
    {
        [BsonId]
        public string Id { get; set; }

        [BsonElement("t")]
        public IEnumerable<string> Tags { get; set; }

        [BsonElement("r")]
        public int DefaultRange { get; set; }

        [BsonElement("c")]
        public int DefaultCost { get; set; }

        [BsonElement("cd")]
        public float DefaultCd { get; set; }

        [BsonElement("m")]
        public float DefaultMod { get; set; }

        [BsonElement("o")]
        public bool MeleeOnly { get; set; }

        [BsonElement("a")]
        public IEnumerable<string> Actions { get; set; }
    }
}