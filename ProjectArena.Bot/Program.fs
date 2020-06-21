open ProjectArena.Bot.Processors.ConfigurationProcessor
open ProjectArena.Bot.Processors.StorageSetupProcessor
open ProjectArena.Bot.Processors.GameConnectionProcessor
open ProjectArena.Bot.Processors.LearningProcessor
open System
open ProjectArena.Bot.Processors.ExtraProcessor

[<EntryPoint>]
let main argv =
    setupConfiguration()
    |> setupStorage
    |> setupGameConnection
    |> startExtraProcessing
    // |> startLearning  TODO Return after extra processing tests
    |> ignore
    Console.ReadLine() |> ignore
    0
