module ProjectArena.Bot.Processors.StorageSetupProcessor
open System.Reflection
open Microsoft.Extensions.DependencyInjection
open ProjectArena.Infrastructure.Mongo
open ProjectArena.Bot.Models.Configuration
open Microsoft.Extensions.Options
open ProjectArena.Bot.Domain.BotMongoContext

let private setConnectionStringSettings (configuration: StorageConfiguration) =
    let connectionSettings = MongoConnectionSettings();
    connectionSettings.ConnectionString <- configuration.ConnectionString
    Options.Create(connectionSettings)

let private setServiceProvider (configuration: StorageConfiguration) =
    let services = ServiceCollection();
    let botContextSettings = MongoContextSettings<BotContext>();
    botContextSettings.NamespaceName <- configuration.Namespace
    services.AddSingleton(Options.Create(botContextSettings)) |> ignore
    services.BuildServiceProvider()

let private setStorageVariables (configuration: StorageConfiguration) =
    printfn "Loading storage..."
    (setConnectionStringSettings configuration, setServiceProvider configuration)

let private setStorageConnection variables =
    let connectionSettings, provider = variables
    MongoConnection (Assembly.GetExecutingAssembly(), connectionSettings, provider) :> IMongoConnection

let private initializeDefaultModels (numberOfValue: int) connection: IMongoConnection =
    printfn "Loading default models..."
    let context = BotContext(connection)
    // TODO Setup neural models with random
    connection

let setupStorage (configuration: RawConfiguration) =
    let mongoConnection =
        configuration.Storage
        |> setStorageVariables
        |> setStorageConnection
        |> initializeDefaultModels configuration.Learning.SuccessfulModelsAmount
    {
        RawConfigurationWithStorageConnection.Learning = configuration.Learning
        Api = configuration.Api
        Storage = mongoConnection
    }