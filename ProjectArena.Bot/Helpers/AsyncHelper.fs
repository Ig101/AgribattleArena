module ProjectArena.Bot.Helpers.AsyncHelper
open System
open System.Threading.Tasks
open System.Threading

let processSequenceAsynchronously (maxTasks: int) (func: 'a -> Async<'b>) (sequence: 'a seq)  : Async<'b list> = async {
    let mutable queue = sequence |> Seq.toList
    let mutable hasTasksToProcess = true
    let mutable tasks: Task<'b> list = []
    let mutable results = []
    while hasTasksToProcess do
        let neededCount = maxTasks - tasks.Length
        let newTasksFromQueue = match neededCount with
                                | c when c < queue.Length -> queue |> List.take c
                                | _ ->
                                    hasTasksToProcess <- false
                                    queue
        queue <- queue |> List.filter (fun q -> not (newTasksFromQueue |> Seq.contains q))
        let startedTasks =
            newTasksFromQueue
            |> List.map (func >> Async.StartAsTask)
        tasks <- tasks |> List.append startedTasks
        do! Task.WhenAny tasks
        results <- tasks |> List.filter (fun t -> t.IsCompleted) |> List.map (fun t -> t.Result) |> List.append results
        tasks <- tasks |> List.filter (fun t -> not t.IsCompleted)
    let! remainedResults =
        Task.WhenAll tasks
    return remainedResults |> Seq.append results |> Seq.toList
}