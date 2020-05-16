using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using ProjectArena.Infrastructure.Enums;

namespace ProjectArena.Domain.Registry.Entities
{
    public class TalentNode
    {
        [BsonId]
        public int Position { get; set; }

        [BsonElement("u")]
        public string Id { get; set; }

        [BsonElement("n")]
        public string Name { get; set; }

        [BsonElement("t")]
        public CharacterClass? Class { get; set; }

        [BsonElement("p")]
        public int ClassPoints { get; set; }

        [BsonElement("sa")]
        public int SkillsAmount { get; set; }

        [BsonElement("d")]
        public string UniqueDescription { get; set; }

        [BsonElement("a")]
        public string UniqueAction { get; set; }

        [BsonElement("s")]
        public int StrengthModifier { get; set; }

        [BsonElement("w")]
        public int WillpowerModifier { get; set; }

        [BsonElement("c")]
        public int ConstitutionModifier { get; set; }

        [BsonElement("i")]
        public int SpeedModifier { get; set; }

        [BsonElement("e")]
        public IEnumerable<string> Exceptions { get; set; }
    }
}