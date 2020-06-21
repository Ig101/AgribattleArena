namespace ProjectArena.Bot.Models.States
open ProjectArena.Bot.Models.Dtos
open System

type Scene = {
    Id: Guid
    Version: int
    RoundsPassed: float32
    TempActor: ActorDto option
    TempDecoration: ActiveDecorationDto option
    Players: PlayerDto seq
    Actors: ActorDto seq
    Decorations: ActiveDecorationDto seq
    Effects: SpecEffectDto seq
    Tiles: TileDto seq
    TilesetWidth: int
    TilesetHeight: int
}