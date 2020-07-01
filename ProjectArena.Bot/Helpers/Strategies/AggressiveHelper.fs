module ProjectArena.Bot.Helpers.Strategies.AggressiveHelper
open ProjectArena.Bot.Models.Dtos
open ProjectArena.Bot.Models.Neural
open ProjectArena.Bot.Models.States
open ProjectArena.Bot.Helpers.Strategies.StrategyHelper

let getAggressiveAction (scene: Scene, actor: ActorDto) =
    SceneAction.Wait