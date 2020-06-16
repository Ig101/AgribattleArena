namespace ProjectArena.Bot.Domain.BotMongoContext.Entities
open MongoDB.Bson.Serialization.Attributes;

// TODO Implement
type NeuralModel = {
    [<BsonId>]
    Id : string
}