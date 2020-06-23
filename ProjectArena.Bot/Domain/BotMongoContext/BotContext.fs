namespace ProjectArena.Bot.Domain.BotMongoContext
open ProjectArena.Infrastructure.Mongo
open ProjectArena.Bot.Domain.BotMongoContext.Entities

type BotContext(connection:IMongoConnection) =
    inherit BaseMongoContext(connection)

    member this.NeuralModels = base.InitializeRepository<NeuralModel>();
    member this.NeuralModelDefinitions = base.InitializeRepository<NeuralModelDefinition>();