namespace ProjectArena.Bot.Worker.Functors
open System.Collections.Generic
open FSharp.Control
open ProjectArena.Bot.Models.Dtos
open ProjectArena.Bot.Models.States
open System.Threading

type SceneState =
    private {
        Dict: IDictionary<string, AutoResetEvent * IncomingSynchronizationMessage>
        CancellationFunction: IncomingSynchronizationMessage -> bool
    }

    member this.SendAndReturnProcessorIfCreated message =
        let sceneId = message.Synchronizer.Id
        let queueGettingSuccess = this.Dict.ContainsKey sceneId
        match queueGettingSuccess with
        | false -> 
            let handle = new AutoResetEvent false
            this.Dict.Add(sceneId, (handle, message))
            let seq = asyncSeq {
                let mutable stateCheck = true
                while stateCheck do
                    do! Async.AwaitWaitHandle handle |> Async.Ignore
                    let _, message = this.Dict.[sceneId]
                    stateCheck <- not (message |> this.CancellationFunction)
                    yield message
            }
            handle.Set() |> ignore
            Some seq
        | true ->
            let handle, _ = this.Dict.[sceneId]
            this.Dict.[sceneId] <- (handle, message)
            handle.Set() |> ignore
            None


    member this.RemoveScene sceneId =
        let handle, _ = this.Dict.[sceneId]
        this.Dict.Remove sceneId |> ignore
        handle.Dispose()

    static member Unit (cancellationFunction: IncomingSynchronizationMessage -> bool) =
        {
            Dict = Dictionary<string, AutoResetEvent * IncomingSynchronizationMessage>() 
            CancellationFunction = cancellationFunction
        }