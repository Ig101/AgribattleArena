using System;
using System.Collections.Generic;
using System.Linq;
using ProjectArena.Engine.Helpers;
using ProjectArena.Engine.Natives;
using ProjectArena.Engine.Objects.Abstract;

namespace ProjectArena.Engine.Objects.Immaterial
{
    public class Skill : IdObject
    {
        private readonly Actor parent;
        private readonly float cd;
        private readonly int range;

        public string Visualization { get; set; }

        public string EnemyVisualization { get; set; }

        public int Range => range + (range > 1 ? parent.BuffManager.SkillRange : 0);

        public SkillNative Native { get; }

        public float Cd => cd * parent.Initiative;

        public float Mod { get; }

        public float PreparationTime { get; set; }

        public IEnumerable<string> AggregatedTags => Native.Tags.Concat(parent.Tags);

        public bool Revealed { get; set; }

        public Skill(Actor parent, SkillNative skill, string visualization, string enemyVisualization, float? cd, float? mod, int? cost, int? range)
            : base(parent.Parent)
        {
            this.Visualization = visualization ?? skill.DefaultVisualization;
            this.EnemyVisualization = enemyVisualization ?? skill.DefaultEnemyVisualization;
            this.range = range ?? skill.DefaultRange;
            this.Mod = mod ?? skill.DefaultMod;
            this.cd = cd ?? skill.DefaultCd;
            this.Native = skill;
            this.parent = parent;
            this.Revealed = false;
        }

        public void Update()
        {
            if (PreparationTime > 0)
            {
                PreparationTime -= 1;
            }
        }

        public bool Cast(Tile target)
        {
            if (PreparationTime <= 0 && parent.BuffManager.CanAct &&
                    ((Native.AvailableTargets.Allies && target.TempObject != null && target.TempObject is Actor && target.TempObject != parent && target.TempObject.Owner?.Team == parent.Owner?.Team) ||
                    (Native.AvailableTargets.NotAllies && target.TempObject != null && target.TempObject is Actor && target.TempObject != parent && (parent.Owner?.Team == null || target.TempObject.Owner?.Team != parent.Owner?.Team)) ||
                    (Native.AvailableTargets.Decorations && target.TempObject != null && target.TempObject is ActiveDecoration) ||
                    (Native.AvailableTargets.Self && target.TempObject == parent) ||
                    (Native.AvailableTargets.Bearable && target.TempObject == null && !target.Native.Unbearable) ||
                    (Native.AvailableTargets.Unbearable && target.TempObject == null && target.Native.Unbearable)) &&
                Misc.RangeBetween(parent.X, parent.Y, target.X, target.Y) <= range)
            {
                parent.TempTile.Native.OnActionAction?.Invoke(parent.Parent, parent.TempTile);
                parent.OnCastAction?.Invoke(parent.Parent, parent);
                Native.Action(parent.Parent, parent, target, this);
                PreparationTime = cd;
                Revealed = true;
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
