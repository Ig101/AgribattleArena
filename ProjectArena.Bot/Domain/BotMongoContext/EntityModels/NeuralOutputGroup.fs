namespace ProjectArena.Bot.Domain.BotMongoContext.EntityModels
open MongoDB.Bson.Serialization.Attributes;

type NeuralOutputGroup = {
    [<BsonElement("o")>]
    Output: string
    [<BsonElement("s")>]
    Shift: float
    [<BsonElement("i")>]
    Inputs: NeuralBond seq
}