namespace ProjectArena.Bot.Domain.BotMongoContext.EntityModels
open MongoDB.Bson.Serialization.Attributes;

type NeuralLayer = {
    [<BsonElement("i")>]
    SortIndex: sbyte
    [<BsonElement("o")>]
    Outputs: NeuralOutputGroup seq
}