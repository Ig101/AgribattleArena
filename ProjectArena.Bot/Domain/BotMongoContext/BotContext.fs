namespace ProjectArena.Bot.Domain.BotMongoContext
open ProjectArena.Infrastructure.Mongo

type BotContext(connection:IMongoConnection) =
    inherit BaseMongoContext(connection)

    member this.NeuralModels = base.InitializeRepository<ProjectArena.Bot.Domain.BotMongoContext.Entities.NeuralModel>();