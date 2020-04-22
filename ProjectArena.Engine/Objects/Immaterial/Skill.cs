using System.Collections.Generic;
using System.Linq;
using AgribattleArena.Engine.Helpers;
using AgribattleArena.Engine.Natives;
using AgribattleArena.Engine.Objects.Abstract;

namespace AgribattleArena.Engine.Objects.Immaterial
{
    public class Skill : IdObject
    {
        private readonly IActorParentRef parent;
        private readonly float cd;
        private readonly int cost;
        private readonly int range;

        public int Range => range + (range > 1 ? parent.BuffManager.SkillRange : 0);

        public SkillNative Native { get; }

        public float Cd => cd + (cd > 0 ? parent.BuffManager.SkillCd : 0);

        public float Mod { get; }

        public int Cost => cost + parent.BuffManager.SkillCost;

        public float PreparationTime { get; set; }

        public IEnumerable<string> AggregatedTags => Native.Tags.Concat(parent.Tags);

        public Skill(IActorParentRef parent, SkillNative skill, float? cd, float? mod, int? cost, int? range)
            : base(parent.Parent)
        {
            this.range = range ?? skill.DefaultRange;
            this.Mod = mod ?? skill.DefaultMod;
            this.cost = cost ?? skill.DefaultCost;
            this.cd = cd ?? skill.DefaultCd;
            this.Native = skill;
            this.parent = parent;
        }

        public void Update(float time)
        {
            if (PreparationTime > 0)
            {
                PreparationTime -= time;
            }
        }

        public bool Cast(Tile target)
        {
            if (parent.ActionPoints >= cost && PreparationTime <= 0 && parent.BuffManager.CanAct &&
                Misc.RangeBetween(parent.X, parent.Y, parent.TempTile.Height, target.X, target.Y, target.Height) <= range)
            {
                Native.Action(parent.Parent, parent, target, this);
                PreparationTime = cd;
                parent.SpendActionPoints(cost);
                return true;
            }

            return false;
        }

        public float CalculateMod(string[] targetTags)
        {
            float tempMod = this.Mod;
            foreach (string attackTag in AggregatedTags)
            {
                foreach (string defenceTag in targetTags)
                {
                    foreach (TagSynergy synergy in parent.AttackModifiers)
                    {
                        if ((synergy.SelfTag == attackTag || synergy.SelfTag == null) && (synergy.TargetTag == defenceTag || synergy.TargetTag == null) &&
                            !(synergy.SelfTag == null && synergy.TargetTag == null))
                        {
                            tempMod *= synergy.Mod;
                        }
                    }
                }
            }

            return tempMod;
        }

        public float CalculateModSkillPower(string[] targetTags)
        {
            return CalculateMod(targetTags) * parent.SkillPower;
        }

        public float CalculateModAttackPower(string[] targetTags)
        {
            return CalculateMod(targetTags) * parent.AttackPower;
        }
    }
}
