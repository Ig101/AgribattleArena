namespace ProjectArena.Bot.Models.States

type SceneWithMetaData = {
    SceneId: string
    Content: Scene option
    SynchronizersInQueue: IncomingSynchronizationMessage list
}