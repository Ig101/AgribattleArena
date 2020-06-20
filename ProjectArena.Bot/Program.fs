open ProjectArena.Bot.Processors.ConfigurationProcessor
open ProjectArena.Bot.Processors.StorageSetupProcessor
open ProjectArena.Bot.Processors.GameConnectionProcessor
open ProjectArena.Bot.Processors.LearningProcessor
open System

[<EntryPoint>]
let main argv =
    setupConfiguration()
    |> setupStorage
    |> setupGameConnection
    |> startLearning
    Console.ReadLine() |> ignore
    0
