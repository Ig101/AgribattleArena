namespace ProjectArena.Bot.Models.Dtos

type ActiveDecorationDto = {
    Id: int
    NativeId: string
    Mod: float
    InitiativePosition: float
    Health: float option
    OwnerId: string option
    IsAlive: bool
    X: int
    Y: int
    Z: float
    MaxHealth: float option
    Armor: TagSynergyDto seq
}