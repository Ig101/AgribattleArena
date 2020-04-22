using System.Collections.Generic;

namespace AgribattleArena.Engine.ForExternalUse.Generation.ObjectInterfaces
{
    public interface IActor
    {
        long? ExternalId { get; }

        string NativeId { get; }

        string AttackingSkillName { get; }

        int Strength { get; }

        int Willpower { get; }

        int Constitution { get; }

        int Speed { get; }

        IEnumerable<string> SkillNames { get; }

        int ActionPointsIncome { get; }

        IEnumerable<string> StartBuffs { get; }
    }
}
