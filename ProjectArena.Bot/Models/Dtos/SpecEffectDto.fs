namespace ProjectArena.Bot.Models.Dtos

type SpecEffectDto = {
    Id: int
    OwnerId: string option
    IsAlive: bool
    X: int
    Y: int
    Z: float
    Duration: float option
    Mod: float
    NativeId: string
}