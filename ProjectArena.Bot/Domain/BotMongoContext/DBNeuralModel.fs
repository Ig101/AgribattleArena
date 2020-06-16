namespace ProjectArena.Bot.Domain.BotMongoContext
open ProjectArena.Infrastructure.Mongo
open ProjectArena.Bot.Models.NeuralModels

// TODO Implement
type NeuralModelMonad =
    private {
        MongoConnection: IMongoConnection
        Id: string
    }

    static member Bind func (model: NeuralModelMonad) =
        0

    static member Unit (connection: IMongoConnection) (id: string) =
        0