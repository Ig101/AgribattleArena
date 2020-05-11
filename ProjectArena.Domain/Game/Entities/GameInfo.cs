using System;
using MongoDB.Bson.Serialization.Attributes;

namespace ProjectArena.Domain.Game.Entities
{
    public class GameInfo
    {
        [BsonId]
        public string Id { get; set; }

        [BsonElement("a")]
        public bool Actual { get; set; }

        [BsonElement("d")]
        public DateTime LastUpdateDate { get; set; }
    }
}