module ProjectArena.Bot.Processors.AuthorizationProcessor
open System.Threading
open ProjectArena.Bot.Models.Configuration
open ProjectArena.Bot.Domain.GameConnection.SignalRConnection
open ProjectArena.Bot.Domain.GameConnection.GameApi
open ProjectArena.Bot.Domain.SceneStateWorker
open ProjectArena.Bot.Models.States
open Microsoft.AspNetCore.SignalR.Client
open FSharp.Control
open ProjectArena.Infrastructure.Mongo

let private authorize (configuration: ApiConfiguration) =
    printfn "Authorizing..."
    let auth =
        configuration.Host
        |> authorize configuration.Login configuration.Password
        |> Async.RunSynchronously
    (configuration, auth)
  
let private getUserId (configuration: ApiConfiguration, auth: string) =
    printfn "Loading User info..."
    let userId =
        configuration.Host
        |> getUserInfo auth
        |> Async.RunSynchronously
    (auth, userId.Id)

let setupAuthorization (configuration: RawConfigurationWithStorageConnection) =
    let auth, userId =
        configuration.Api
        |> authorize
        |> getUserId
    {
        Learning = configuration.Learning
        ApiHost = configuration.Api.Host
        HubPath = configuration.Api.HubPath
        User = {
            UserId = userId
            AuthCookie = auth
        }
        Storage = configuration.Storage
    }