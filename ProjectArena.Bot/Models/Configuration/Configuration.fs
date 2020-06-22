namespace ProjectArena.Bot.Models.Configuration
open ProjectArena.Infrastructure.Mongo
open ProjectArena.Bot.Domain.SceneStateWorker
open Microsoft.AspNetCore.SignalR.Client
open System.Threading
open Microsoft.Extensions.Logging

type Configuration = {
    Logger: ILogger<unit>
    Learning: LearningConfiguration
    User: UserConfiguration
    ApiHost: string
    Hub: HubConnection
    Storage: IMongoConnection
    Worker: SceneStateWorker
    WorkerCancellationToken: CancellationToken
}