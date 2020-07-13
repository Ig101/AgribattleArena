module ProjectArena.Bot.Processors.ConfigurationProcessor
open System
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Logging
open Serilog
open ProjectArena.Bot.Models.Configuration
open Serilog.Events

let private newConfigurationBuilder() =
    let builder = ConfigurationBuilder() :> IConfigurationBuilder
    builder

let private setBasePath directory (builder: IConfigurationBuilder) =
    builder.SetBasePath(directory)

let private addJsonFile name (builder: IConfigurationBuilder) =
    builder.AddJsonFile(name, true, true)

let private addEnvironmentVariables (builder: IConfigurationBuilder) =
    builder.AddEnvironmentVariables()

let private build (builder: IConfigurationBuilder) =
    builder.Build()

let private setupLogger level =
    let logger = LoggerConfiguration().Enrich.FromLogContext().WriteTo.Console().MinimumLevel.Is(level).CreateLogger()
    LoggerFactory.Create(fun builder -> builder.AddSerilog(logger) |> ignore).CreateLogger<unit>()

let setupConfiguration() =
    let environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    let configuration =
        newConfigurationBuilder()
        |> setBasePath Environment.CurrentDirectory
        |> addJsonFile "appsettings.json" 
        |> addJsonFile (sprintf "appsettings.%s.json" environment) 
        |> addEnvironmentVariables
        |> build
    {
        RawConfiguration.Learning = {
            IsLearning = configuration.GetValue<bool>("Learning:IsLearning")
            BatchSize = configuration.GetValue<int>("Learning:BatchSize")
            ModelsAmount = configuration.GetValue<int>("Learning:ModelsAmount")
            SuccessfulModelsAmount = configuration.GetValue<int>("Learning:SuccessfulModelsAmount")
            MutationProbability = configuration.GetValue<float>("Learning:MutationProbability")
            TimeTillSurrender = configuration.GetValue<float32>("Learning:TimeTillSurrender")
            
            VictoryPerformanceCoefficient = configuration.GetValue<float>("Learning:VictoryPerformanceCoefficient")
            EnemyPowerPerformanceCoefficient = configuration.GetValue<float>("Learning:EnemyPowerPerformanceCoefficient")
            PlayerPowerPerformanceCoefficient = configuration.GetValue<float>("Learning:PlayerPowerPerformanceCoefficient")
            
            HiddenNeuronsCount = configuration.GetValue<int>("Learning:HiddenNeuronsCount")
            ActivationDivider = configuration.GetValue<float>("Learning:ActivationDivider")
        }
        LazyNeuralModels = configuration.GetValue<bool>("LazyNeuralModels")
        Logger = setupLogger(configuration.GetValue<LogEventLevel>("Logger:Level"))
        Api = {
            Host = configuration.GetValue<string>("Api:Host")
            HubPath = configuration.GetValue<string>("Api:HubPath")
            Login = configuration.GetValue<string>("Api:Login")
            Password = configuration.GetValue<string>("Api:Password")
        }
        Storage = {
            ConnectionString = configuration.GetValue<string>("Storage:ConnectionString")
            Namespace = configuration.GetValue<string>("Storage:Namespace")
        }
    }