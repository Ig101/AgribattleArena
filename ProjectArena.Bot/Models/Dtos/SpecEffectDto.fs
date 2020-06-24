namespace ProjectArena.Bot.Models.Dtos

type SpecEffectDto = {
    Id: int
    OwnerId: string option
    IsAlive: bool
    X: int
    Y: int
    Z: float32
    Duration: float32 option
    NativeId: string
    Visualization: string
}