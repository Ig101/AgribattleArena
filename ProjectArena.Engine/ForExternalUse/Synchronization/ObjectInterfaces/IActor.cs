using System;
using System.Collections.Generic;
using ProjectArena.Engine.Helpers;

namespace ProjectArena.Engine.ForExternalUse.Synchronization.ObjectInterfaces
{
    public interface IActor
    {
        int Id { get; }

        Guid? ExternalId { get; }

        string NativeId { get; }

        ISkill AttackingSkill { get; }

        int Strength { get; }

        int Willpower { get; }

        int Constitution { get; }

        int Speed { get; }

        List<ISkill> Skills { get; }

        int ActionPointsIncome { get; }

        List<IBuff> Buffs { get; }

        float InitiativePosition { get; }

        float Health { get; }

        string OwnerId { get; }

        int? Team { get; }

        bool IsAlive { get; }

        int X { get; }

        int Y { get; }

        float Z { get; }

        int MaxHealth { get; }

        int ActionPoints { get; }

        float SkillPower { get; }

        float AttackPower { get; }

        float Initiative { get; }

        List<TagSynergy> Armor { get; }

        List<TagSynergy> AttackModifiers { get; }

        bool CanMove { get; }

        bool CanAct { get; }

        bool HealthRevealed { get; }
    }
}
