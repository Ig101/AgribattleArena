namespace ProjectArena.Bot.Models.Configuration
open ProjectArena.Infrastructure.Mongo
open Microsoft.Extensions.Logging

type RawConfigurationWithStorageAndUser = {
    Logger: ILogger<unit>
    Learning: LearningConfiguration
    LazyNeuralModels: bool
    User: UserConfiguration
    ApiHost: string
    HubPath: string
    Storage: IMongoConnection
}