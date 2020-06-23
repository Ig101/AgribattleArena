module ProjectArena.Bot.Processors.AuthorizationProcessor
open Microsoft.Extensions.Logging
open ProjectArena.Bot.Models.Configuration
open ProjectArena.Bot.Domain.GameConnection.GameApi
open FSharp.Control

let private authorize (configuration: RawConfigurationWithStorageConnection) =
    configuration.Logger.LogInformation "Authorizing..."
    let auth =
        configuration.Api.Host
        |> authorize configuration.Logger configuration.Api.Login configuration.Api.Password
        |> Async.RunSynchronously
    auth
  
let private getUserId (configuration: RawConfigurationWithStorageConnection) (auth: string) =
    configuration.Logger.LogInformation "Loading User info..."
    let userId =
        configuration.Api.Host
        |> getUserInfo configuration.Logger auth
        |> Async.RunSynchronously
    (auth, userId.Id)

let setupAuthorization (configuration: RawConfigurationWithStorageConnection) =
    let auth, userId =
        authorize configuration
        |> getUserId configuration
    {
        LazyNeuralModels = configuration.LazyNeuralModels
        Logger = configuration.Logger
        Learning = configuration.Learning
        ApiHost = configuration.Api.Host
        HubPath = configuration.Api.HubPath
        User = {
            UserId = userId
            AuthCookie = auth
        }
        Storage = configuration.Storage
    }