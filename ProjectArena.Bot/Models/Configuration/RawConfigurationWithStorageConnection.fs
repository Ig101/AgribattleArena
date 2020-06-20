namespace ProjectArena.Bot.Models.Configuration
open ProjectArena.Infrastructure.Mongo

type RawConfigurationWithStorageConnection = {
    Learning: LearningConfiguration
    Api: ApiConfiguration
    Storage: IMongoConnection
}