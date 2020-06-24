namespace ProjectArena.Bot.Models.Dtos

type SkillDto = {
    Id: int
    Range: int
    NativeId: string
    Visualization: string
    Cd: float32
    Cost: int
    PreparationTime: float32
    AvailableTargets: TargetsDto
    OnlyVisibleTargets: bool
}