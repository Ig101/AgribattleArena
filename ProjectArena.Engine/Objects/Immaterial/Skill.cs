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
        private readonly IActorParentRef parent;
        private readonly float cd;
        private readonly int cost;
        private readonly int range;

        public string Visualization { get; set; }

        public string EnemyVisualization { get; set; }

        public int Range => range + (range > 1 ? parent.BuffManager.SkillRange : 0);

        public SkillNative Native { get; }

        public float Cd => cd + (cd > 0 ? parent.BuffManager.SkillCd : 0);

        public float Mod { get; }

        public int Cost => cost + parent.BuffManager.SkillCost;

        public float PreparationTime { get; set; }

        public IEnumerable<string> AggregatedTags => Native.Tags.Concat(parent.Tags);

        public bool Revealed { get; set; }

        public Skill(IActorParentRef parent, SkillNative skill, string visualization, string enemyVisualization, float? cd, float? mod, int? cost, int? range)
            : base(parent.Parent)
        {
            this.Visualization = visualization ?? skill.DefaultVisualization;
            this.EnemyVisualization = enemyVisualization ?? skill.DefaultEnemyVisualization;
            this.range = range ?? skill.DefaultRange;
            this.Mod = mod ?? skill.DefaultMod;
            this.cost = cost ?? skill.DefaultCost;
            this.cd = cd ?? skill.DefaultCd;
            this.Native = skill;
            this.parent = parent;
            this.Revealed = false;
        }

        public void Update(float time)
        {
            if (PreparationTime > 0)
            {
                PreparationTime -= time;
            }
        }

        private bool CheckMilliness(Tile target)
        {
            var range = Misc.RangeBetween(parent.X, parent.Y, target.X, target.Y);
            var incrementingRange = 0;
            var angleBetween = Misc.AngleBetween(parent.X, parent.Y, target.X, target.Y);
            var sin = Math.Sin(angleBetween);
            var cos = Math.Cos(angleBetween);
            var currentTile = parent.TempTile;
            while (incrementingRange <= range)
            {
                incrementingRange++;
                Tile nextTarget;
                if (incrementingRange >= range)
                {
                    nextTarget = target;
                }
                else
                {
                    var nextXFloat = parent.X + (incrementingRange * cos);
                    var nextYFloat = parent.Y + (incrementingRange * sin);
                    var nextX = (int)Math.Round(nextXFloat, MidpointRounding.AwayFromZero);
                    var nextY = (int)Math.Round(nextYFloat, MidpointRounding.AwayFromZero);
                    nextTarget = parent.Parent.Tiles[nextX][nextY];
                    if (nextTarget == currentTile)
                    {
                        continue;
                    }
                }

                if (nextTarget.Height - currentTile.Height >= 10 ||
                    (Range <= 1 && currentTile.Height - nextTarget.Height >= 10) ||
                    (nextTarget != target && nextTarget.TempObject != null) ||
                    nextTarget.Native.Unbearable)
                {
                    return false;
                }

                currentTile = nextTarget;
            }

            return true;
        }

        public bool Cast(Tile target)
        {
            if (parent.ActionPoints >= cost && PreparationTime <= 0 && parent.BuffManager.CanAct &&
                    ((Native.AvailableTargets.Allies && target.TempObject != null && target.TempObject is Actor && target.TempObject != parent && target.TempObject.Owner?.Team == parent.Owner?.Team) ||
                    (Native.AvailableTargets.NotAllies && target.TempObject != null && target.TempObject is Actor && target.TempObject != parent && (parent.Owner?.Team == null || target.TempObject.Owner?.Team != parent.Owner?.Team)) ||
                    (Native.AvailableTargets.Decorations && target.TempObject != null && target.TempObject is ActiveDecoration) ||
                    (Native.AvailableTargets.Self && target.TempObject == parent) ||
                    (Native.AvailableTargets.Bearable && target.TempObject == null && !target.Native.Unbearable) ||
                    (Native.AvailableTargets.Unbearable && target.TempObject == null && target.Native.Unbearable)) &&
                (!Native.OnlyVisibleTargets || CheckMilliness(target)) &&
                Misc.RangeBetween(parent.X, parent.Y, target.X, target.Y) <= range)
            {
                Native.Action(parent.Parent, parent, target, this);
                PreparationTime = cd;
                parent.SpendActionPoints(cost);
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
