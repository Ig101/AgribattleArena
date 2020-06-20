module ProjectArena.Bot.Processors.ConfigurationProcessor
open System
open Microsoft.Extensions.Configuration
open ProjectArena.Bot.Models.Configuration

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

let setupConfiguration() =
    printfn "Loading configuration..."
    let environment = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");
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
            ModelsAmount = configuration.GetValue<int>("Learning:ModelsAmount")
            SuccessfulModelsAmount = configuration.GetValue<int>("Learning:SuccessfulModelsAmount")
            LearningMutationProbability = configuration.GetValue<double>("Learning:LearningMutationProbability")
            WorkingMutationProbability = configuration.GetValue<double>("Learning:WorkingMutationProbability")
        }
        Api = {
            Host = configuration.GetValue<string>("Api:Host")
            HubPath = configuration.GetValue<string>("Api:HubPath")
            Login = configuration.GetValue<string>("Learning:Login")
            Password = configuration.GetValue<string>("Learning:Password")
        }
        Storage = {
            ConnectionString = configuration.GetValue<string>("Storage:ConnectionString")
            Namespace = configuration.GetValue<string>("Storage:Namespace")
        }
    }