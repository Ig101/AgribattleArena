using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace ProjectArena.Domain.Game.Entities
{
    public class Character
    {
        [BsonId]
        public string Id { get; set; }

        [BsonElement("d")]
        public bool Deleted { get; set; }

        [BsonElement("k")]
        public bool IsKeyCharacter { get; set; }

        [BsonElement("r")]
        public string RosterId { get; set; }

        [BsonElement("n")]
        public string Name { get; set; }

        [BsonElement("t")]
        public IEnumerable<int> ChosenTalents { get; set; }
    }
}