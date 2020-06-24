namespace ProjectArena.Bot.Models.Dtos

type ActiveDecorationDto = {
    Id: int
    NativeId: string
    Visualization: string
    InitiativePosition: float32
    Health: float32 option
    OwnerId: string option
    IsAlive: bool
    X: int
    Y: int
    Z: float32
    MaxHealth: float32 option
}