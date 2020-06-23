namespace ProjectArena.Bot.Domain.BotMongoContext.Entities
open MongoDB.Bson.Serialization.Attributes;

type NeuralModelDefinition = {
    [<BsonId>]
    Id : string
    [<BsonElement("k")>]
    Key: bool
}