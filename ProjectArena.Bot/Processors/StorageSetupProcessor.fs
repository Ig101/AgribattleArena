module ProjectArena.Bot.Processors.StorageSetupProcessor
open System.Reflection
open Microsoft.Extensions.DependencyInjection
open ProjectArena.Infrastructure.Mongo
open ProjectArena.Bot.Models.Configuration
open Microsoft.Extensions.Options
open Microsoft.Extensions.Logging
open ProjectArena.Bot.Domain.BotMongoContext
open ProjectArena.Bot.Processors.NeuralProcessor
open ProjectArena.Bot.Domain.BotMongoContext.Entities

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

let private setStorageVariables (configuration: RawConfiguration) =
    configuration.Logger.LogInformation "Loading storage..."
    (setConnectionStringSettings configuration.Storage, setServiceProvider configuration.Storage)

let private setStorageConnection variables =
    let connectionSettings, provider = variables
    MongoConnection (Assembly.GetExecutingAssembly(), connectionSettings, provider) :> IMongoConnection

let private initializeDefaultModels (configuration: RawConfiguration) connection: IMongoConnection =
    let insertNewModels (context: BotContext) (models: NeuralModel seq) =
        match models |> Seq.length with
        | 0 -> ()
        | _ -> 
            context.NeuralModels.Insert models
            context.ApplyChangesAsync().Wait()

    configuration.Logger.LogInformation "Loading default models..."
    let context = BotContext(connection)
    let modelsAmount =
        context.NeuralModels.GetAsync(fun _ -> true).Result
        |> Seq.length

    [1..configuration.Learning.SuccessfulModelsAmount - modelsAmount]
    |> Seq.map(fun _ -> initializeRandomNeuralModel())
    |> insertNewModels context

    connection

let setupStorage (configuration: RawConfiguration) =
    let mongoConnection =
        setStorageVariables configuration
        |> setStorageConnection
        |> initializeDefaultModels configuration
    {
        RawConfigurationWithStorageConnection.Learning = configuration.Learning
        Logger = configuration.Logger
        Api = configuration.Api
        Storage = mongoConnection
    }