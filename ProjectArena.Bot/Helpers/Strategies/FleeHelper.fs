module ProjectArena.Bot.Helpers.Strategies.FleeHelper
open ProjectArena.Bot.Models.Dtos
open ProjectArena.Bot.Models.Neural
open ProjectArena.Bot.Models.States
open ProjectArena.Bot.Helpers.Strategies.StrategyHelper

let getFleeAction (scene: Scene, actor: ActorDto) =
    SceneAction.Move (1, 1)