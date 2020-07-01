module ProjectArena.Bot.Helpers.Strategies.DefenciveHelper
open ProjectArena.Bot.Models.Dtos
open ProjectArena.Bot.Models.Neural
open ProjectArena.Bot.Models.States
open ProjectArena.Bot.Helpers.Strategies.StrategyHelper

let getDefenciveAction (scene: Scene, actor: ActorDto) =
    SceneAction.Wait