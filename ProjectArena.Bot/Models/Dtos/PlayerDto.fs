namespace ProjectArena.Bot.Models.Dtos
open ProjectArena.Infrastructure.Enums

type PlayerDto = {
    Id: string
    UserId: string
    Team: int option
    KeyActorsSync: int seq
    TurnsSkipped: int
    Status: PlayerStatus
}