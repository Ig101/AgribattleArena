// Learn more about F# at http://fsharp.org

open System
open ProjectArena.Bot.Domain.GameState.SceneStateProcessor
open ProjectArena.Bot.Models.States
open ProjectArena.Bot.Models.Dtos

[<EntryPoint>]
let main argv =

    let processor (scene: Scene, synchronizers) (action, synchronizer: SynchronizerDto) =
        let newScene = { scene with
                           RoundsPassed = scene.RoundsPassed + synchronizer.RoundsPassed
                       }
        printfn "Action %A for scene %s with new time %A. New scene: %A" action scene.Id synchronizer.RoundsPassed newScene
        (newScene, synchronizers)

    let finalProcessor (scene: Scene) =
        printfn "Scene %s processing ended" scene.Id
        scene.Id

    let sceneState = globalSceneState
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
        addMessageToState sceneState processor finalProcessor (action, {Id = sceneId; RoundsPassed = time})
        res + 1) 0
