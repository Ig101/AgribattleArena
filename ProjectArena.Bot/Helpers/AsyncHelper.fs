module ProjectArena.Bot.Helpers.AsyncHelper
open System
open System.Threading.Tasks
open System.Threading

let processSequenceAsynchronously (func: 'a -> Async<'b>) (sequence: 'a seq)  : Async<'b seq> =
    let tasks =
        sequence
        |> Seq.map (func >> Async.StartAsTask)
    Task.WhenAll tasks
    |> Async.AwaitTask
    |> Async.map Seq.ofArray