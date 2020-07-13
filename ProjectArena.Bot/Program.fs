open ProjectArena.Bot.Processors.ConfigurationProcessor
open ProjectArena.Bot.Processors.StorageSetupProcessor
open ProjectArena.Bot.Processors.GameConnectionProcessor
open ProjectArena.Bot.Processors.LearningProcessor
open System
open ProjectArena.Bot.Processors.ExtraProcessor
open ProjectArena.Bot.Processors.AuthorizationProcessor
open System.Threading

[<EntryPoint>]
let main argv =
    setupConfiguration()
    |> setupStorage
    |> setupAuthorization
    |> setupGameConnection
    |> startExtraProcessing
    |> startLearning
    |> ignore
    Thread.Sleep(Timeout.Infinite)
    0
