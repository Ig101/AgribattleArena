namespace ProjectArena.Bot.Domain.BotMongoContext.Entities
open MongoDB.Bson.Serialization.Attributes;
open ProjectArena.Bot.Domain.BotMongoContext.EntityModels

type NeuralModel = {
    [<BsonId>]
    Id : string
    [<BsonElement("l")>]
    Layers : NeuralLayer seq
}