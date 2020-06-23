namespace ProjectArena.Bot.Models.Configuration
open ProjectArena.Infrastructure.Mongo
open Microsoft.Extensions.Logging

type RawConfigurationWithStorageConnection = {
    Logger: ILogger<unit>
    Learning: LearningConfiguration
    LazyNeuralModels: bool
    Api: ApiConfiguration
    Storage: IMongoConnection
}