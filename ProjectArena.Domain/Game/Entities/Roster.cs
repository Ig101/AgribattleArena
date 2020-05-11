using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace ProjectArena.Domain.Game.Entities
{
    public class Roster
    {
        [BsonId]
        [BsonElement("id")]
        public string UserId { get; set; }

        [BsonElement("t")]
        public DateTime LastTimeRefreshed { get; set; }

        [BsonElement("e")]
        public int Experience { get; set; }

        [BsonElement("s")]
        public int Seed { get; set; }

        [BsonElement("c")]
        public int TavernCapacity { get; set; }

        [BsonElement("p")]
        public IEnumerable<int> BoughtPatrons { get; set; }
    }
}