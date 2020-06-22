module ProjectArena.Bot.Processors.NeuralProcessor
open System
open ProjectArena.Bot.Domain.BotMongoContext.Entities
open ProjectArena.Bot.Models.States
open Microsoft.AspNetCore.SignalR.Client
open ProjectArena.Bot.Domain.GameConnection.SignalRConnection

let initializeRandomNeuralModel() =
    // TODO Setup neural models with random
    {
        NeuralModel.Id = Guid.NewGuid().ToString()
    }

let actOnScene (hub: HubConnection) (model: NeuralModel, scene: Scene) =
    printfn "doAction with Model: %A" model.Id
    scene.TempActor
    |> Option.map(fun a -> orderWait hub (scene.Id, a.Id))
    |> ignore
    