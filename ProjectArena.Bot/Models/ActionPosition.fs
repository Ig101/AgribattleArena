namespace ProjectArena.Bot.Models.Neural

type ActionPosition = {
    X: int
    Y: int
    Steps: int
    IsDecorationOnWay: bool
    AllowedActionsWithPriority: (SceneAction * float) list
    RangeTillNearestEnemy: float
    RangeTillNearestAlly: float
    ParentSquares: ActionPosition list
}