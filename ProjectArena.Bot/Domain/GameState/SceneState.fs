namespace ProjectArena.Bot.Domain.GameState
open System.Collections.Generic
open FSharp.Control
open ProjectArena.Bot.Models.Dtos
open ProjectArena.Bot.Models.States
open ProjectArena.Bot.Domain.GameSignalRConnection
open System.Threading

type SceneState =
    {
        Dict: Dictionary<string, AutoResetEvent * SynchronizationAction * SynchronizerDto>
        CancellationFunction: (SynchronizationAction * SynchronizerDto) -> bool
    }

    member this.SendAndReturnProcessorIfCreated (action, synchronizer: SynchronizerDto) =
        let sceneId = synchronizer.Id
        let queueGettingSuccess = this.Dict.ContainsKey sceneId
        match queueGettingSuccess with
        | false -> 
            let handle = new AutoResetEvent false
            this.Dict.Add(sceneId, (handle, action, synchronizer))
            let seq = asyncSeq {
                let mutable stateCheck = true
                while stateCheck do
                    do! Async.AwaitWaitHandle handle |> Async.Ignore
                    let _, action, synchronizer = this.Dict.[sceneId]
                    stateCheck <- not ((action, synchronizer) |> this.CancellationFunction)
                    yield (action, synchronizer)
            }
            handle.Set() |> ignore
            Some seq
        | true ->
            let handle, _, _ = this.Dict.[sceneId]
            this.Dict.[sceneId] <- (handle, action, synchronizer)
            handle.Set() |> ignore
            None


    member this.RemoveScene sceneId =
        let handle, _, _ = this.Dict.[sceneId]
        this.Dict.Remove sceneId |> ignore
        handle.Dispose()

    static member Unit (cancellationFunction: (SynchronizationAction * SynchronizerDto) -> bool) =
        {
            Dict = Dictionary<string, AutoResetEvent * SynchronizationAction * SynchronizerDto>() 
            CancellationFunction = cancellationFunction
        }