// Learn more about F# at http://fsharp.org

open System
open ProjectArena.Bot.Models.States
open ProjectArena.Bot.Models.Dtos
open ProjectArena.Bot.Worker.Worker

[<EntryPoint>]
let main argv =

    let processor (scene: SceneWithMetaData) (message: IncomingSynchronizationMessage) =
        let newContent = match scene.Content with
                         | Some c -> {
                             c with RoundsPassed = c.RoundsPassed + message.Synchronizer.RoundsPassed
                            }
                         | None -> { RoundsPassed = message.Synchronizer.RoundsPassed }
        let newScene = { scene with
                           Content = Some newContent
                       }
        printfn "Action %A for scene %s with new time %A. New scene: %A" message.Action scene.SceneId message.Synchronizer.RoundsPassed newScene
        newScene

    let finalProcessor (scene: SceneWithMetaData) =
        printfn "Scene %s processing ended" scene.SceneId
        scene.SceneId

    seq {
        while true do
            Console.ReadLine()
    }
    |> Seq.map(fun str ->
        let strings = str.Split(" ")
        let time = strings.[2] |> Double.Parse
        let action = match strings.[0] with
                     | "e" -> EndGame
                     | _ -> EndTurn
        (action, strings.[1], time))
    |> Seq.fold(fun res (action, sceneId, time) ->
        sendMessageToGlobalState processor finalProcessor { Action = action; Synchronizer = {Id = sceneId; RoundsPassed = time} }
        res + 1) 0
