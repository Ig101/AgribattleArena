using System.Collections.Generic;
using AgribattleArena.Engine.Helpers;

namespace AgribattleArena.Engine.Objects.Immaterial.Buffs
{
    public interface IBuffManagerParentRef
    {
        int SkillCd { get; set; }

        int SkillCost { get; set; }

        int SkillRange { get; set; }

        bool CanMove { get; set; }

        bool CanAct { get; set; }

        IActorParentRef Parent { get; set; }

        float AdditionInitiative { get; set; }

        float AttackPower { get; set; }

        float SkillPower { get; set; }

        float AdditionMaxHealth { get; set; }

        float AdditionActionPointsIncome { get; set; }

        float AdditionStrength { get; set; }

        float AdditionWillpower { get; set; }

        float AdditionConstitution { get; set; }

        float AdditionSpeed { get; set; }

        List<TagSynergy> Armor { get; }

        List<TagSynergy> Attack { get; }

        List<Buff> Buffs { get; }
    }
}
