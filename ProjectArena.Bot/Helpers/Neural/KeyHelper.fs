module ProjectArena.Bot.Helpers.Neural.KeyHelper
open System
open ProjectArena.Bot.Models.Neural
open ProjectArena.Bot.Models.States
open ProjectArena.Bot.Helpers.SceneHelper
open ProjectArena.Bot.Models.Dtos

let private getHarmful (currentActor: ActorDto) (actor: ActorDto option) =
    actor |> Option.map (fun a -> Math.Min (1.0, getActorPower a / getActorPower currentActor * strengthDifferenceCoefficient)) |> Option.defaultValue 0.0

let private getHelpful (actor: ActorDto option) =
    actor |> Option.map getActorPower |> Option.defaultValue 0.0

let private getKeyValue (shiftX: int, shiftY: int) (index: int, t: KeyNeuronType) (scene: Scene) =
    scene |> tryGetCurrentActorAndHisOwner
    |> Option.map (fun (actor, owner) ->
        match index with
        | index when index < 6 ->
            // Enemies
            let enemyActorId = (scene.Players |> Seq.find (fun p -> p.Id <> owner.Id)).KeyActorsSync |> Seq.item index
            let enemyActor = scene.Actors |> Seq.tryFind (fun a -> a.Id = enemyActorId)
            match t with
            | KeyNeuronType. XShift -> getXShift shiftX enemyActor
            | KeyNeuronType.YShift -> getYShift shiftX enemyActor
            | Harmful -> getHarmful actor enemyActor
            | Helpful -> 0.0
            | KeyNeuronType.Vulnerable -> getVulnerable enemyActor
            | KeyNeuronType.Damaged -> getDamaged enemyActor
            | Known -> getKnown enemyActor
            | Alive -> match enemyActor with | Some _ -> 1.0 | None -> 0.0
            | KeyNeuronType.Mobile -> getMobile enemyActor
            | KeyNeuronType.Tough -> getTough enemyActor
        | _ ->
            // Friends
            let friendActorId = owner.KeyActorsSync |> Seq.filter (fun a -> a <> actor.Id) |> Seq.item (index - 6)
            let friendActor = scene.Actors |> Seq.tryFind (fun a -> a.Id = friendActorId)
            match t with
            | KeyNeuronType. XShift -> getXShift shiftX friendActor
            | KeyNeuronType.YShift -> getYShift shiftX friendActor
            | Harmful -> 0.0
            | Helpful -> getHelpful friendActor
            | KeyNeuronType.Vulnerable -> getVulnerable friendActor
            | KeyNeuronType.Damaged -> getDamaged friendActor
            | Known -> 1.0
            | Alive -> match friendActor with | Some _ -> 1.0 | None -> 0.0
            | KeyNeuronType.Mobile -> getMobile friendActor
            | KeyNeuronType.Tough -> getTough friendActor
    ) |> Option.defaultValue 0.0

let private getKeyNeuron (shift: int * int) (scene: Scene option) (index: int) =
    [ KeyNeuronType.XShift; KeyNeuronType.YShift; Harmful; Helpful; KeyNeuronType.Vulnerable; KeyNeuronType.Damaged; Known; Alive; KeyNeuronType.Mobile; KeyNeuronType.Tough ]
    |> List.map (fun t ->
        let modifier = match t with
                       | KeyNeuronType.XShift -> "x"
                       | KeyNeuronType.YShift -> "y"
                       | Harmful -> "e"
                       | Helpful -> "h"
                       | KeyNeuronType.Vulnerable -> "v"
                       | KeyNeuronType.Damaged -> "d"
                       | Known -> "k"
                       | Alive -> "a"
                       | KeyNeuronType.Mobile -> "m"
                       | KeyNeuronType.Tough -> "t"
        {
            Name = sprintf "k%0i%s" index modifier
            Value = scene |> Option.map (getKeyValue shift (index, t)) |> Option.defaultValue 0.0
        })

let getKeyInputNeurons (shift: int * int) (scene: Scene option) =
    [0..10]
    |> List.collect (fun k -> k |> getKeyNeuron shift scene)

let getMagnifyingKeyInputNeurons (scene: Scene option) =
    getKeyInputNeurons (0, 0) scene