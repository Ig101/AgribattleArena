namespace ProjectArena.Bot.Models.Dtos

type TileDto = {
    X: int
    Y: int
    TempActorId: int option
    Height: float32
    NativeId: string
    OwnerId: string option
    Unbearable: bool
}