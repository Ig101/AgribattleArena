using System;
using System.Collections.Generic;
using ProjectArena.Engine.Helpers;
using ProjectArena.Engine.Objects;
using ProjectArena.Engine.Objects.Abstract;
using ProjectArena.Engine.Objects.Immaterial;
using ProjectArena.Engine.Objects.Immaterial.Buffs;

namespace ProjectArena.Engine.ForExternalUse
{
    public interface INativeManager
    {
        void AddActorNative(
            string id,
            string defaultVisualization,
            string defaultEnemyVisualization,
            string[] tags,
            float defaultZ,
            TagSynergy[] armor,
            Action<Scene, Actor, float> onHitAction,
            Action<Scene, Actor> onCastAction,
            Action<Scene, Actor> onDeathAction);

        void AddBuffNative(
            string id,
            string[] tags,
            bool eternal,
            int repeatable,
            bool summarizeLength,
            int? defaultDuration,
            float defaultMod,
            Action<BuffManager, Buff> applier,
            Action<Scene, Actor, Buff> onPurgeAction);

        void AddDecorationNative(
            string id,
            string defaultVisualization,
            string[] tags,
            TagSynergy[] defaultArmor,
            int defaultHealth,
            float defaultZ,
            float defaultMod,
            Action<Scene, ActiveDecoration, float> onHitAction,
            Action<Scene, ActiveDecoration> onDeathAction);

        void AddRoleModelNative(
            string id,
            int defaultStrength,
            int defaultWillpower,
            int defaultConstitution,
            int defaultSpeed,
            string attackingSkill,
            string[] skills);

        void AddSkillNative(
            string id,
            string defaultVisualization,
            string defaultEnemyVisualization,
            string[] tags,
            int defaultRange,
            float defaultCd,
            float defaultMod,
            Targets availableTargets,
            Action<Scene, Actor, Tile, Skill> action);

        void AddTileNative(
            string id,
            string[] tags,
            bool flat,
            int defaultHeight,
            bool unbearable,
            float defaultMod,
            Action<Scene, Tile, TileObject> onCreateAction,
            Action<Scene, Tile> onActionAction,
            Action<Scene, Tile, TileObject> onDeathAction);
    }
}
