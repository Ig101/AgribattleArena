module ProjectArena.Bot.Worker.Worker
open ProjectArena.Bot.Models.Dtos
open Microsoft.FSharp.Core
open ProjectArena.Bot.Models.States
open FSharp.Control.AsyncSeqExtensions
open FSharp.Control
open System.Threading
open System.Threading.Tasks
open ProjectArena.Bot.Worker.Functors

let sendMessageToGlobalState = 
    let globalSceneState =
        SceneState.Unit (fun message -> message.Action = EndGame || message.Action = NoActorsDraw)

    let sendMessageToState
        (sceneState: SceneState)
        (processor: SceneWithMetaData -> IncomingSynchronizationMessage -> SceneWithMetaData)
        (resultProcessor: SceneWithMetaData -> string)
        (synchronizer: IncomingSynchronizationMessage) =

        synchronizer
        |> sceneState.SendAndReturnProcessorIfCreated
        |> Option.iter (
            AsyncSeq.fold processor { SceneId = synchronizer.Synchronizer.Id; Content = None; SynchronizersInQueue = []}
            >> Async.map resultProcessor
            >> Async.map sceneState.RemoveScene
            >> Async.Start
        )
    sendMessageToState globalSceneState