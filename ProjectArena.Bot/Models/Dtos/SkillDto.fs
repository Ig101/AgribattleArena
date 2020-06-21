namespace ProjectArena.Bot.Models.Dtos

type SkillDto = {
    Id: int
    Range: int
    NativeId: string
    Cd: float
    Cost: int
    PreparationTime: float
    AvailableTargets: TargetsDto
    OnlyVisibleTargets: bool
}