using System;
using System.Collections.Generic;
using ProjectArena.Engine.Helpers;
using ProjectArena.Engine.Natives;
using ProjectArena.Engine.Objects;
using ProjectArena.Engine.Objects.Immaterial;
using ProjectArena.Engine.Objects.Immaterial.Buffs;

namespace ProjectArena.Engine.NativeManagers
{
    public class NativeManager : INativeManager, ForExternalUse.INativeManager
    {
        private readonly Dictionary<string, ActorNative> actorNatives;
        private readonly Dictionary<string, ActiveDecorationNative> decorationNatives;
        private readonly Dictionary<string, BuffNative> buffNatives;
        private readonly Dictionary<string, SpecEffectNative> effectNatives;
        private readonly Dictionary<string, SkillNative> skillNatives;
        private readonly Dictionary<string, RoleModelNative> roleModelNatives;
        private readonly Dictionary<string, TileNative> tileNatives;

        public NativeManager()
        {
            actorNatives = new Dictionary<string, ActorNative>();
            decorationNatives = new Dictionary<string, ActiveDecorationNative>();
            buffNatives = new Dictionary<string, BuffNative>();
            effectNatives = new Dictionary<string, SpecEffectNative>();
            skillNatives = new Dictionary<string, SkillNative>();
            roleModelNatives = new Dictionary<string, RoleModelNative>();
            tileNatives = new Dictionary<string, TileNative>();
        }

        private string[] InternTags(string[] tags)
        {
            string[] newTags = new string[tags.Length];
            for (int i = 0; i < tags.Length; i++)
            {
                newTags[i] = string.Intern(tags[i]);
            }

            return newTags;
        }

        public void AddActorNative(string id, string[] tags, float defaultZ, TagSynergy[] armor)
        {
            actorNatives.Add(id, new ActorNative(id, InternTags(tags), defaultZ, armor));
        }

        public void AddBuffNative(
            string id,
            string[] tags,
            bool eternal,
            int repeatable,
            bool summarizeLength,
            bool onTile,
            int? defaultDuration,
            float defaultMod,
            Action<ISceneParentRef, IActorParentRef, Buff, float> action,
            Action<IBuffManagerParentRef, Buff> applier,
            Action<ISceneParentRef, IActorParentRef, Buff> onPurgeAction)
        {
            buffNatives.Add(id, new BuffNative(id, InternTags(tags), eternal, repeatable, summarizeLength, onTile, defaultDuration, defaultMod, action, applier, onPurgeAction));
        }

        public void AddDecorationNative(
            string id,
            string[] tags,
            TagSynergy[] defaultArmor,
            int defaultHealth,
            float defaultZ,
            float defaultMod,
            Action<ISceneParentRef, ActiveDecoration> action,
            Action<ISceneParentRef, ActiveDecoration> onDeathAction)
        {
            decorationNatives.Add(id, new ActiveDecorationNative(id, InternTags(tags), defaultArmor, defaultHealth, defaultZ, defaultMod, action, onDeathAction));
        }

        public void AddEffectNative(
            string id,
            string[] tags,
            float defaultZ,
            float? defaultDuration,
            float defaultMod,
            Action<ISceneParentRef, SpecEffect, float> action,
            Action<ISceneParentRef, SpecEffect> onDeathAction)
        {
            effectNatives.Add(id, new SpecEffectNative(id, InternTags(tags), defaultZ, defaultDuration, defaultMod, action, onDeathAction));
        }

        public void AddRoleModelNative(
            string id,
            int defaultStrength,
            int defaultWillpower,
            int defaultConstitution,
            int defaultSpeed,
            int defaultActionPointsIncome,
            string attackingSkill,
            string[] skills)
        {
            roleModelNatives.Add(id, new RoleModelNative(
                this,
                id,
                defaultStrength,
                defaultWillpower,
                defaultConstitution,
                defaultSpeed,
                defaultActionPointsIncome,
                attackingSkill,
                skills));
        }

        public void AddSkillNative(
            string id,
            string[] tags,
            int defaultRange,
            int defaultCost,
            float defaultCd,
            float defaultMod,
            Targets availableTargets,
            bool onlyVisibleTargets,
            Action<ISceneParentRef, IActorParentRef, Tile, Skill> action)
        {
            skillNatives.Add(id, new SkillNative(id, InternTags(tags), defaultRange, defaultCost, defaultCd, defaultMod, availableTargets, onlyVisibleTargets, action));
        }

        public void AddTileNative(
            string id,
            string[] tags,
            bool flat,
            int defaultHeight,
            bool unbearable,
            float defaultMod,
            bool revealedByDefault,
            Action<ISceneParentRef, Tile, float> action,
            Action<ISceneParentRef, Tile> onStepAction)
        {
            tileNatives.Add(id, new TileNative(id, InternTags(tags), flat, defaultHeight, unbearable, defaultMod, revealedByDefault, action, onStepAction));
        }

        public ActorNative GetActorNative(string id)
        {
            ActorNative native = actorNatives[id];
            return native;
        }

        public BuffNative GetBuffNative(string id)
        {
            BuffNative native = buffNatives[id];
            return native;
        }

        public ActiveDecorationNative GetDecorationNative(string id)
        {
            ActiveDecorationNative native = decorationNatives[id];
            return native;
        }

        public SpecEffectNative GetEffectNative(string id)
        {
            SpecEffectNative native = effectNatives[id];
            return native;
        }

        public RoleModelNative GetRoleModelNative(string id)
        {
            RoleModelNative native = roleModelNatives[id];
            return native;
        }

        public SkillNative GetSkillNative(string id)
        {
            SkillNative native = skillNatives[id];
            return native;
        }

        public TileNative GetTileNative(string id)
        {
            TileNative native = tileNatives[id];
            return native;
        }
    }
}
