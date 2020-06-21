namespace ProjectArena.Bot.Models.Dtos
open System

type ActorDto = {
    Id: int
    ExternalId: Guid option
    NativeId: string
    AttackingSkill: SkillDto option
    Skills: SkillDto seq
    Buffs: BuffDto seq
    InitiativePosition: float32
    Health: float32 option
    OwnerId: string option
    X: int
    Y: int
    Z: float32
    MaxHealth: int option
    ActionPoints: int
    Initiative: float32
    CanMove: bool
    CanAct: bool
}