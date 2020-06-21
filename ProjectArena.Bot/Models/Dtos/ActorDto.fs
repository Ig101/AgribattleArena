namespace ProjectArena.Bot.Models.Dtos
open System

type ActorDto = {
    Id: int
    ExternalId: Guid option
    NativeId: string
    AttackingSkill: SkillDto option
    Skills: SkillDto seq
    Buffs: BuffDto seq
    InitiativePosition: float
    Health: float option
    OwnerId: string option
    X: int
    Y: int
    Z: float
    MaxHealth: int option
    ActionPoints: int
    Initiative: float
    CanMove: bool
    CanAct: bool
}