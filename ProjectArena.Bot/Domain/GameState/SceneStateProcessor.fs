module ProjectArena.Bot.Domain.GameState.SceneStateProcessor
open ProjectArena.Bot.Models.Dtos
open Microsoft.FSharp.Core
open ProjectArena.Bot.Models.States
open FSharp.Control.AsyncSeqExtensions
open FSharp.Control
open System.Threading
open System.Threading.Tasks

let globalSceneState =
    SceneState.Unit (fun (action, _) -> action = EndGame || action = NoActorsDraw)

let addMessageToState
    (sceneState: SceneState)
    (processor:Scene * SynchronizerDto list -> SynchronizationAction * SynchronizerDto -> Scene * SynchronizerDto list)
    (resultProcessor: Scene -> string)
    (synchronizer: SynchronizationAction * SynchronizerDto) =

    let pipeline = sceneState.SendAndReturnProcessorIfCreated synchronizer
    let _, sync = synchronizer
    pipeline
    |> Option.iter (
        AsyncSeq.fold processor ({ Id = sync.Id; RoundsPassed = 0.0 }, [])
        >> Async.map (fun (scene, _) -> scene)
        >> Async.map resultProcessor
        >> Async.map sceneState.RemoveScene
        >> Async.Start
    )
    ()