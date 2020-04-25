using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using ProjectArena.Domain.Registry.EntityModels;

namespace ProjectArena.Domain.Registry.Entities
{
    public class DecorationNative
    {
        [BsonId]
        public string Id { get; set; }

        [BsonElement("t")]
        public IEnumerable<string> Tags { get; set; }

        [BsonElement("da")]
        public IEnumerable<TagSynergy> DefaultArmor { get; set; }

        [BsonElement("h")]
        public int DefaultHealth { get; set; }

        [BsonElement("z")]
        public float DefaultZ { get; set; }

        [BsonElement("m")]
        public float DefaultMod { get; set; }

        [BsonElement("a")]
        public IEnumerable<string> Actions { get; set; }

        [BsonElement("d")]
        public IEnumerable<string> OnDeathActions { get; set; }
    }
}