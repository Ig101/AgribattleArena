module ProjectArena.Bot.Processors.NeuralProcessor
open System
open ProjectArena.Bot.Domain.BotMongoContext.Entities
open ProjectArena.Bot.Domain.BotMongoContext.EntityModels
open ProjectArena.Bot.Models.States
open Microsoft.AspNetCore.SignalR.Client
open ProjectArena.Bot.Domain.GameConnection.SignalRConnection
open ProjectArena.Bot.Models.Configuration
open ProjectArena.Bot.Helpers.Neural.NeuralHelper
open ProjectArena.Bot.Domain.BotMongoContext
open ProjectArena.Bot.Functors
open System.Threading.Tasks

let calculateNeededAction (configuration: Configuration) (model: NeuralModelContainer, scene: Scene) = async {
    let! workingModel = model.Unpack()
    return 0
}

let actOnScene (configuration: Configuration) (model: NeuralModelContainer, scene: Scene) = async {
    let! action = calculateNeededAction configuration (model, scene)
    GC.Collect()
    scene.TempActor
    |> Option.map(fun a -> orderWait configuration.Hub (scene.Id, a.Id))
    |> ignore
}