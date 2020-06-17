namespace ProjectArena.Bot.Domain.BotMongoContext
open ProjectArena.Infrastructure.Mongo
open ProjectArena.Bot.Models.NeuralModels

// TODO Implement
type DBNeuralModel =
    private {
        MongoConnection: IMongoConnection
        Id: string
    }

    static member Bind func (model: DBNeuralModel) =
        0

    static member Unit (connection: IMongoConnection) (id: string) =
        0