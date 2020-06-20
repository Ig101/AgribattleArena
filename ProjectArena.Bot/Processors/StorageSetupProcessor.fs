module ProjectArena.Bot.Processors.StorageSetupProcessor
open System.Reflection
open Microsoft.Extensions.DependencyInjection
open ProjectArena.Infrastructure.Mongo
open ProjectArena.Bot.Models.Configuration
open Microsoft.Extensions.Options
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

let private setStorageVariables (configuration: StorageConfiguration) =
    printfn "Loading storage..."
    (setConnectionStringSettings configuration, setServiceProvider configuration)

let private setStorageConnection variables =
    let connectionSettings, provider = variables
    MongoConnection (Assembly.GetExecutingAssembly(), connectionSettings, provider) :> IMongoConnection

let private initializeDefaultModels (numberOfValue: int) connection: IMongoConnection =
    let insertNewModels (context: BotContext) (models: NeuralModel seq) =
        match models |> Seq.length with
        | 0 -> ()
        | _ -> 
            context.NeuralModels.Insert models
            context.ApplyChangesAsync().Wait()

    printfn "Loading default models..."
    let context = BotContext(connection)
    let modelsAmount =
        context.NeuralModels.GetAsync(fun _ -> true).Result
        |> Seq.length

    [1..numberOfValue - modelsAmount]
    |> Seq.map(fun _ -> initializeRandomNeuralModel())
    |> insertNewModels context

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