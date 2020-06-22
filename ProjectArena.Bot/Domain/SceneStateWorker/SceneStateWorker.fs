namespace ProjectArena.Bot.Domain.SceneStateWorker
open System.Collections.Generic
open FSharp.Control
open ProjectArena.Bot.Models.Dtos
open ProjectArena.Bot.Models.States
open System.Threading
open System

type SceneStateWorker =
    private {
        SceneIdWithSubscribeAndReceiveHandlesAndMessage: IDictionary<Guid, AutoResetEvent * AutoResetEvent * IncomingSynchronizationMessage>
        CancellationFunction: IncomingSynchronizationMessage -> bool
        SubscribeHandle: AutoResetEvent
        ExtraSubscribeHandle: AutoResetEvent
        ReceiveHandle: AutoResetEvent
        Locker: obj
        mutable SubscriptionObject: AsyncSeq<IncomingSynchronizationMessage> option
        mutable ActiveSubscriptions: int
    }

    member private this.TryGetNewScene() =
        this.SubscriptionObject
        |> Option.map(fun sub ->
            this.SubscriptionObject <- None
            this.ReceiveHandle.Set() |> ignore
            sub)

    member private this.RemoveScene sceneId =
        let subscribeHandle, receiveHandle, _ = this.SceneIdWithSubscribeAndReceiveHandlesAndMessage.[sceneId]
        this.SceneIdWithSubscribeAndReceiveHandlesAndMessage.Remove sceneId |> ignore
        subscribeHandle.Dispose()
        receiveHandle.Dispose()

    member this.SendNewMessage message =
        async {
            let sceneId = message.Synchronizer.Id
            let queueGettingSuccess = lock this.Locker (fun () ->
                let queueGettingSuccess = this.SceneIdWithSubscribeAndReceiveHandlesAndMessage.ContainsKey sceneId
                if not queueGettingSuccess then
                    let subscribeHandle = new AutoResetEvent false
                    let receiveHandle = new AutoResetEvent false
                    this.SceneIdWithSubscribeAndReceiveHandlesAndMessage.Add(sceneId, (subscribeHandle, receiveHandle, message))
                queueGettingSuccess)
            match queueGettingSuccess with
            | false -> 
                do! Async.AwaitWaitHandle this.ReceiveHandle |> Async.Ignore
                let subscribeHandle, receiveHandle, _ = this.SceneIdWithSubscribeAndReceiveHandlesAndMessage.[sceneId]
                let seq = asyncSeq {
                    let mutable stateCheck = true
                    while stateCheck do
                        let! waitingSuccess = Async.AwaitWaitHandle (subscribeHandle, 240000)
                        match waitingSuccess with
                        | false ->
                            stateCheck <- false
                            receiveHandle.Set() |> ignore
                        | true ->
                            let _, _, message = this.SceneIdWithSubscribeAndReceiveHandlesAndMessage.[sceneId]
                            stateCheck <- not (message |> this.CancellationFunction)
                            receiveHandle.Set() |> ignore
                            yield message
                    this.RemoveScene sceneId
                }
                this.SubscriptionObject <- Some seq
                this.SubscribeHandle.Set() |> ignore
                this.ExtraSubscribeHandle.Set() |> ignore
                subscribeHandle.Set() |> ignore
            | true ->
                let subscribeHandle, receiveHandle, _ = this.SceneIdWithSubscribeAndReceiveHandlesAndMessage.[sceneId]
                do! Async.AwaitWaitHandle receiveHandle |> Async.Ignore
                this.SceneIdWithSubscribeAndReceiveHandlesAndMessage.[sceneId] <- (subscribeHandle, receiveHandle, message)
                subscribeHandle.Set() |> ignore
        } |> Async.Start
        ()

    member this.GetNextNewScene() =
        let incrementActiveSubs() =
            this.ActiveSubscriptions <- this.ActiveSubscriptions + 1
        let decrementActiveSubs() =
            this.ActiveSubscriptions <- this.ActiveSubscriptions - 1
        async {
            lock this.Locker incrementActiveSubs
            let mutable scene = None
            let tryGetNewScene() =
                scene <- this.TryGetNewScene()
            while scene = None do
                do! Async.AwaitWaitHandle this.SubscribeHandle |> Async.Ignore
                lock this.Locker tryGetNewScene
            lock this.Locker decrementActiveSubs
            return scene.Value
        }

    member this.GetNextNewExtraScene() =
        async {
            let mutable scene = None
            let tryGetNewScene() =
                scene <- this.TryGetNewScene()
            while scene = None do
                do! Async.AwaitWaitHandle this.ExtraSubscribeHandle |> Async.Ignore
                printfn "Active subs: %d" this.ActiveSubscriptions
                while this.ActiveSubscriptions > 0 do
                    do! Async.AwaitWaitHandle this.ExtraSubscribeHandle |> Async.Ignore
                lock this.Locker tryGetNewScene
            return scene.Value
        }

    static member Unit() =
        {
            SceneIdWithSubscribeAndReceiveHandlesAndMessage = Dictionary<Guid, AutoResetEvent * AutoResetEvent * IncomingSynchronizationMessage>() 
            CancellationFunction = fun message -> message.Action = EndGame
            SubscribeHandle = new AutoResetEvent false
            ExtraSubscribeHandle = new AutoResetEvent false
            ReceiveHandle = new AutoResetEvent true
            SubscriptionObject = None
            ActiveSubscriptions = 0
            Locker = new obj()
        }