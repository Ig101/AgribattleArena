namespace ProjectArena.Bot.Models.Neural

type ActionPosition = {
    X: int
    Y: int
    Steps: int
    IsDecorationOnWay: bool
    AllyAllowedActionWithPriority: (SceneAction * float) option
    EnemyAllowedActionWithPriority: (SceneAction * float) option
    RangeTillNearestEnemy: float
    RangeTillNearestAlly: float
    ParentSquares: ActionPosition list
}