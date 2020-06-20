open ProjectArena.Bot.Processors.ConfigurationProcessor
open ProjectArena.Bot.Processors.StorageSetupProcessor
open ProjectArena.Bot.Processors.GameConnectionProcessor

[<EntryPoint>]
let main argv =
    let worker =
        setupConfiguration()
        |> setupStorage
        |> setupGameConnection
    0
