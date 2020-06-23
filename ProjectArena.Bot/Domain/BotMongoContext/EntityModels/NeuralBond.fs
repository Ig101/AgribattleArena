namespace ProjectArena.Bot.Domain.BotMongoContext.EntityModels
open MongoDB.Bson.Serialization.Attributes;

type NeuralBond = {
    [<BsonElement("i")>]
    Input: string
    [<BsonElement("w")>]
    Weight: float
}