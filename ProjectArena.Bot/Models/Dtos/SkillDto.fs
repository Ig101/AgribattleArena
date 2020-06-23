namespace ProjectArena.Bot.Models.Dtos

type SkillDto = {
    Id: int
    Range: int
    Visualization: string
    Cd: float32
    Cost: int
    PreparationTime: float32
    AvailableTargets: TargetsDto
    OnlyVisibleTargets: bool
}