using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace ProjectArena.Domain.Registry.Entities
{
    public class BuffNative
    {
        [BsonId]
        public string Id { get; set; }

        [BsonElement("t")]
        public IEnumerable<string> Tags { get; set; }

        [BsonElement("e")]
        public bool Eternal { get; set; }

        [BsonElement("r")]
        public int Repeatable { get; set; }

        [BsonElement("s")]
        public bool SummarizeLength { get; set; }

        [BsonElement("d")]
        public int? DefaultDuration { get; set; }

        [BsonElement("m")]
        public float DefaultMod { get; set; }

        [BsonElement("a")]
        public IEnumerable<string> Actions { get; set; }

        [BsonElement("ap")]
        public IEnumerable<string> Appliers { get; set; }

        [BsonElement("p")]
        public IEnumerable<string> OnPurgeActions { get; set; }
    }
}