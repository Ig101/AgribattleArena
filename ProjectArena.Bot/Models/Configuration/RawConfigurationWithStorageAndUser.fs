namespace ProjectArena.Bot.Models.Configuration
open ProjectArena.Infrastructure.Mongo

type RawConfigurationWithStorageAndUser = {
    Learning: LearningConfiguration
    User: UserConfiguration
    ApiHost: string
    HubPath: string
    Storage: IMongoConnection
}