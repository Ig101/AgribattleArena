namespace ProjectArena.Bot.Models.Configuration
open Microsoft.Extensions.Logging

type RawConfiguration = {
    Logger: ILogger<unit>
    Learning: LearningConfiguration
    LazyNeuralModels: bool
    Api: ApiConfiguration
    Storage: StorageConfiguration
}