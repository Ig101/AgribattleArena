module ProjectArena.Bot.Helpers.AsyncHelper
open System
open System.Threading.Tasks
open System.Threading
open FSharp.Control
open System.Collections.Generic

let private toQueue (sequence: 'a seq) =
    let queue = Queue<'a>()
    sequence |> Seq.iter (fun v -> queue.Enqueue(v))
    queue

let processSequenceAsynchronously (maxTasks: int) (func: 'a -> Async<'b>) (sequence: 'a seq)  : Async<'b list> =
    let queue = sequence |> toQueue
    [1..maxTasks]
    |> AsyncSeq.ofSeq
    |> AsyncSeq.mapAsyncParallel(fun _ ->
        async {
            let mutable results = []
            while queue.Count > 0 do
                let input = lock queue (fun () -> queue.Dequeue())
                let! result = func input
                results <- results |> List.append [result]
            return results
        })
    |> AsyncSeq.fold (fun r i -> r |> List.append i) []