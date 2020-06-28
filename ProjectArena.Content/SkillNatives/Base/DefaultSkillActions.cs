using ProjectArena.Engine;
using ProjectArena.Engine.Objects;
using ProjectArena.Engine.Objects.Immaterial;

namespace ProjectArena.Content.SkillNatives.Base
{
    internal static class DefaultSkillActions
    {
        public static void DoDamageAttack(this Tile targetTile, Skill skill)
        {
            if (targetTile.TempObject != null)
            {
                float mod = skill.CalculateModAttackPower(targetTile.TempObject.Native.Tags);
                targetTile.TempObject.Damage(mod, skill.AggregatedTags);
            }
        }

        public static void DoDamageSkill(this Tile targetTile, Skill skill)
        {
            if (targetTile.TempObject != null)
            {
                float mod = skill.CalculateModSkillPower(targetTile.TempObject.Native.Tags);
                targetTile.TempObject.Damage(mod, skill.AggregatedTags);
            }
        }

        public static void AddBuff(this Tile targetTile, string buffName, float mod, float duration, bool dividedByInitiative)
        {
            if (targetTile.TempObject != null && targetTile.TempObject is Actor target)
            {
                var divider = dividedByInitiative ? (target.Initiative * 0.9f) : 1;
                duration /= divider;
                target.BuffManager.AddBuff(buffName, mod, duration / divider);
            }
        }

        public static void AddBuff(this Tile targetTile, string buffName, float mod)
        {
            if (targetTile.TempObject != null && targetTile.TempObject is Actor target)
            {
                target.BuffManager.AddBuff(buffName, mod, null);
            }
        }
    }
}