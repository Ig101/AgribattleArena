using MongoDB.Bson.Serialization.Attributes;

namespace ProjectArena.Domain.Registry.EntityModels
{
    public class TagSynergy
    {
        [BsonElement("s")]
        public string SelfTag { get; }

        [BsonElement("t")]
        public string TargetTag { get; }

        [BsonElement("m")]
        public float Mod { get; }
    }
}