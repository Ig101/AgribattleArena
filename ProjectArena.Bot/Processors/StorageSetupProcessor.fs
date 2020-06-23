module ProjectArena.Bot.Processors.StorageSetupProcessor
open System.Reflection
open Microsoft.Extensions.DependencyInjection
open ProjectArena.Infrastructure.Mongo
open ProjectArena.Bot.Models.Configuration
open Microsoft.Extensions.Options
open Microsoft.Extensions.Logging
open ProjectArena.Bot.Domain.BotMongoContext
open ProjectArena.Bot.Processors.NeuralCreationProcessor
open ProjectArena.Bot.Domain.BotMongoContext.Entities
open System
open MongoDB.Driver

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
    configuration.Logger.LogInformation "Loading default models..."
    let random = Random()
    let context = BotContext(connection)
    let nonKeyIds = context.NeuralModelDefinitions.GetAsync(fun m -> not m.Key).Result |> Seq.map (fun m -> m.Id) |> Seq.toList
    context.NeuralModels.Delete(Builders<NeuralModel>.Filter.In ((fun m -> m.Id), nonKeyIds))
    context.NeuralModelDefinitions.Delete( Builders<NeuralModelDefinition>.Filter.In ((fun m -> m.Id), nonKeyIds))
    context.ApplyChangesAsync().Wait()
    
    let modelsAmount = context.NeuralModelDefinitions.CountAsync(fun m -> m.Key).Result

    [1..configuration.Learning.SuccessfulModelsAmount - modelsAmount]
    |> List.map(fun _ ->
        let model = initializeRandomNeuralModel random configuration.Learning
        context.NeuralModels.InsertOne model
        context.NeuralModelDefinitions.InsertOne {
            Id = model.Id
            Key = true
        }
        context.ApplyChangesAsync().Wait())
    |> ignore

    connection

let setupStorage (configuration: RawConfiguration) =
    let mongoConnection =
        setStorageVariables configuration
        |> setStorageConnection
        |> initializeDefaultModels configuration
    {
        RawConfigurationWithStorageConnection.Learning = configuration.Learning
        LazyNeuralModels = configuration.LazyNeuralModels
        Logger = configuration.Logger
        Api = configuration.Api
        Storage = mongoConnection
    }