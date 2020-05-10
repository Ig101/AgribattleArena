using System;
using System.Collections.Generic;

namespace ProjectArena.Engine.ForExternalUse.Generation.ObjectInterfaces
{
    public interface IActor
    {
        Guid? ExternalId { get; }

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
