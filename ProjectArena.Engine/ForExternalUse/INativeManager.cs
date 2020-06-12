using System;
using System.Collections.Generic;
using ProjectArena.Engine.Helpers;
using ProjectArena.Engine.Objects;
using ProjectArena.Engine.Objects.Immaterial;
using ProjectArena.Engine.Objects.Immaterial.Buffs;

namespace ProjectArena.Engine.ForExternalUse
{
    public interface INativeManager
    {
        void AddActorNative(string id, string[] tags, float defaultZ, TagSynergy[] armor);

        void AddBuffNative(
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
            Action<ISceneParentRef, IActorParentRef, Buff> onPurgeAction);

        void AddDecorationNative(
            string id,
            string[] tags,
            TagSynergy[] defaultArmor,
            int defaultHealth,
            float defaultZ,
            float defaultMod,
            Action<ISceneParentRef, ActiveDecoration> action,
            Action<ISceneParentRef, ActiveDecoration> onDeathAction);

        void AddEffectNative(
            string id,
            string[] tags,
            float defaultZ,
            float? defaultDuration,
            float defaultMod,
            Action<ISceneParentRef, SpecEffect, float> action,
            Action<ISceneParentRef, SpecEffect> onDeathAction);

        void AddRoleModelNative(
            string id,
            int defaultStrength,
            int defaultWillpower,
            int defaultConstitution,
            int defaultSpeed,
            int defaultActionPointsIncome,
            string attackingSkill,
            string[] skills);

        void AddSkillNative(
            string id,
            string[] tags,
            int defaultRange,
            int defaultCost,
            float defaultCd,
            float defaultMod,
            Targets availableTargets,
            bool onlyVisibleTargets,
            Action<ISceneParentRef, IActorParentRef, Tile, Skill> action);

        void AddTileNative(
            string id,
            string[] tags,
            bool flat,
            int defaultHeight,
            bool unbearable,
            float defaultMod,
            bool revealedByDefault,
            Action<ISceneParentRef, Tile, float> action,
            Action<ISceneParentRef, Tile> onStepAction);
    }
}
