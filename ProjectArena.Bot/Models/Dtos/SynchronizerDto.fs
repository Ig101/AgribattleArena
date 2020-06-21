namespace ProjectArena.Bot.Models.Dtos
open System

type SynchronizerDto = {
    Id: Guid
    Version: int
    RoundsPassed: float
    ActorId: int option
    SkillActionId: int option
    TargetX: int option
    TargetY: int option
    TurnTime: float
    TempActor: int option
    TempDecoration: int option
    Players: PlayerDto seq
    ChangedActors: ActorDto seq
    ChangedDecorations: ActiveDecorationDto seq
    ChangedEffects: SpecEffectDto seq
    DeletedActors: int seq
    DeletedDecorations: int seq
    DeletedEffects: int seq
    ChangedTiles: TileDto seq
    TilesetWidth: int
    TilesetHeight: int
}