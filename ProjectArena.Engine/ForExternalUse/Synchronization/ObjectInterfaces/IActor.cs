using System.Collections.Generic;
using AgribattleArena.Engine.Helpers;

namespace AgribattleArena.Engine.ForExternalUse.Synchronization.ObjectInterfaces
{
    public interface IActor
    {
        int Id { get; }

        long? ExternalId { get; }

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
    }
}
