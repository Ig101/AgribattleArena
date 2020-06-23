namespace ProjectArena.Bot.Domain.BotMongoContext.EntityModels
open MongoDB.Bson.Serialization.Attributes;

type NeuralNetwork = {
    // First layer always from input, last layer always to output
    [<BsonElement("l")>]
    Layers : NeuralLayer seq
}