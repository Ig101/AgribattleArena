namespace ProjectArena.Bot.Domain.BotMongoContext.EntityModels
open MongoDB.Bson.Serialization.Attributes;

type NeuralLayer = {
    [<BsonElement("s")>]
    SortIndex: sbyte
    [<BsonElement("o")>]
    Outputs: NeuralOutputGroup seq
}