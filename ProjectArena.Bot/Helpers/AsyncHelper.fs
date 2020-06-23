module ProjectArena.Bot.Helpers.AsyncHelper
open System
open System.Threading.Tasks
open System.Threading

let processSequenceAsynchronously (maxTasks: int) (func: 'a -> Async<'b>) (sequence: 'a seq)  : Async<'b seq> = async {
    let mutable queue = sequence |> Seq.toList
    let mutable hasTasksToProcess = true
    let mutable tasks = []
    while hasTasksToProcess do
        let unfinishedTasks = tasks |> List.filter (fun (t: Task<'b>) -> not t.IsCompleted)
        let neededCount = maxTasks - unfinishedTasks.Length
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
    let! results =
        Task.WhenAll tasks
        |> Async.AwaitTask
    return results |> Seq.ofArray
}