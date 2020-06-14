namespace ProjectArena.Bot.Models.States
open ProjectArena.Bot.Models.Dtos

type IncomingSynchronizationMessage = {
    Action: SynchronizationAction
    Synchronizer: SynchronizerDto
}