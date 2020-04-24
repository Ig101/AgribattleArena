using System.Collections.Generic;
using ProjectArena.Engine.Helpers;
using ProjectArena.Engine.Objects.Immaterial.Buffs;

namespace ProjectArena.Engine.Objects
{
    public interface IActorDamageModelRef
    {
        int MaxHealth { get; }

        List<Buff> Buffs { get; }

        List<TagSynergy> Armor { get; }
    }
}
