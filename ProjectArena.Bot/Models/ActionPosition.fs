namespace ProjectArena.Bot.Models.Neural

type ActionPosition = {
    X: int
    Y: int
    Steps: int
    IsDecorationOnWay: bool
    AllowedActionsWithPriorityAndAlly: (SceneAction * float * bool) list
    RangeTillNearestEnemy: float
    RangeTillNearestAlly: float
    ParentSquares: ActionPosition list
}