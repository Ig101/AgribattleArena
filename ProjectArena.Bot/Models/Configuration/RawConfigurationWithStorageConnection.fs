namespace ProjectArena.Bot.Models.Configuration
open ProjectArena.Infrastructure.Mongo
open Microsoft.Extensions.Logging

type RawConfigurationWithStorageConnection = {
    Logger: ILogger<unit>
    Learning: LearningConfiguration
    Api: ApiConfiguration
    Storage: IMongoConnection
}