module ProjectArena.Bot.Processors.StorageSetupProcessor
open System.Reflection
open Microsoft.Extensions.DependencyInjection
open ProjectArena.Infrastructure.Mongo
open ProjectArena.Bot.Models.Configuration
open Microsoft.Extensions.Options
open ProjectArena.Bot.Domain.BotMongoContext

let private setConnectionStringSettings (configuration: RawConfiguration) =
    let connectionSettings = MongoConnectionSettings();
    connectionSettings.ConnectionString <- configuration.Storage.ConnectionString
    Options.Create(connectionSettings)

let private setServiceProvider (configuration: RawConfiguration) =
    let services = ServiceCollection();
    let botContextSettings = MongoContextSettings<BotContext>();
    botContextSettings.NamespaceName <- configuration.Storage.Namespace
    services.AddSingleton(Options.Create(botContextSettings)) |> ignore
    services.BuildServiceProvider()

let private setStorageVariables (configuration: RawConfiguration) =
    (setConnectionStringSettings configuration, setServiceProvider configuration, configuration.Learning)

let private setStorageConnection variables =
    let connectionSettings, provider, learning = variables
    (MongoConnection (Assembly.GetExecutingAssembly(), connectionSettings, provider) :> IMongoConnection, learning)

let private initializeDefaultModels (variables: IMongoConnection * LearningConfiguration) =
    let connection, learning = variables
    let context = BotContext(connection)
    // TODO Setup neural models with random
    connection

let setupStorage (configuration: RawConfiguration) =
    let mongoConnection =
        configuration
        |> setStorageVariables
        |> setStorageConnection
        |> initializeDefaultModels
    {
        RawConfigurationWithStorageConnection.Learning = configuration.Learning
        Api = configuration.Api
        Storage = mongoConnection
    }