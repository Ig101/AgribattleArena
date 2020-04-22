using System.Collections.Generic;
using AgribattleArena.Engine.Helpers;
using AgribattleArena.Engine.Objects.Immaterial.Buffs;

namespace AgribattleArena.Engine.Objects
{
    public interface IActorDamageModelRef
    {
        int MaxHealth { get; }

        List<Buff> Buffs { get; }

        List<TagSynergy> Armor { get; }
    }
}
