namespace ProjectArena.Bot.Domain.BotMongoContext
open ProjectArena.Infrastructure.Mongo
open ProjectArena.Bot.Models.NeuralModels

type BotContext(connection:IMongoConnection) =
    inherit BaseMongoContext(connection)

    member this.NeuralModels = base.InitializeRepository<NeuralModel>();