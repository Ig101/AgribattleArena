using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace ProjectArena.Domain.Registry.Entities
{
    public class RoleModelNative
    {
        [BsonId]
        public string Id { get; set; }

        [BsonElement("s")]
        public int DefaultStrength { get; set; }

        [BsonElement("w")]
        public int DefaultWillpower { get; set; }

        [BsonElement("c")]
        public int DefaultConstitution { get; set; }

        [BsonElement("v")]
        public int DefaultSpeed { get; set; }

        [BsonElement("p")]
        public int DefaultActionPointsIncome { get; set; }

        [BsonElement("m")]
        public string AttackingSkill { get; set; }

        [BsonElement("a")]
        public IEnumerable<string> Skills { get; set; }
    }
}