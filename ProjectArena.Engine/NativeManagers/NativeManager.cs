using System;
using System.Collections.Generic;
using ProjectArena.Engine.Helpers;
using ProjectArena.Engine.Natives;
using ProjectArena.Engine.Objects;
using ProjectArena.Engine.Objects.Abstract;
using ProjectArena.Engine.Objects.Immaterial;
using ProjectArena.Engine.Objects.Immaterial.Buffs;

namespace ProjectArena.Engine.NativeManagers
{
    public class NativeManager : INativeManager, ForExternalUse.INativeManager
    {
        private readonly Dictionary<string, ActorNative> actorNatives;
        private readonly Dictionary<string, ActiveDecorationNative> decorationNatives;
        private readonly Dictionary<string, BuffNative> buffNatives;
        private readonly Dictionary<string, SkillNative> skillNatives;
        private readonly Dictionary<string, RoleModelNative> roleModelNatives;
        private readonly Dictionary<string, TileNative> tileNatives;

        public NativeManager()
        {
            actorNatives = new Dictionary<string, ActorNative>();
            decorationNatives = new Dictionary<string, ActiveDecorationNative>();
            buffNatives = new Dictionary<string, BuffNative>();
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

        public void AddActorNative(
            string id,
            string defaultVisualization,
            string defaultEnemyVisualization,
            string[] tags,
            float defaultZ,
            TagSynergy[] armor,
            Action<Scene, Actor, float> onHitAction,
            Action<Scene, Actor> onCastAction,
            Action<Scene, Actor> onDeathAction)
        {
            actorNatives.Add(id, new ActorNative(id, defaultVisualization, defaultEnemyVisualization, InternTags(tags), defaultZ, armor, onHitAction, onCastAction, onDeathAction));
        }

        public void AddBuffNative(
            string id,
            string[] tags,
            bool eternal,
            int repeatable,
            bool summarizeLength,
            int? defaultDuration,
            float defaultMod,
            Action<BuffManager, Buff> applier,
            Action<Scene, Actor, Buff> onPurgeAction)
        {
            buffNatives.Add(id, new BuffNative(id, InternTags(tags), eternal, repeatable, summarizeLength, defaultDuration, defaultMod, applier, onPurgeAction));
        }

        public void AddDecorationNative(
            string id,
            string defaultVisualization,
            string[] tags,
            TagSynergy[] defaultArmor,
            int defaultHealth,
            float defaultZ,
            float defaultMod,
            Action<Scene, ActiveDecoration, float> onHitAction,
            Action<Scene, ActiveDecoration> onDeathAction)
        {
            decorationNatives.Add(id, new ActiveDecorationNative(id, defaultVisualization, InternTags(tags), defaultArmor, defaultHealth, defaultZ, defaultMod, onHitAction, onDeathAction));
        }

        public void AddRoleModelNative(
            string id,
            int defaultStrength,
            int defaultWillpower,
            int defaultConstitution,
            int defaultSpeed,
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
                attackingSkill,
                skills));
        }

        public void AddSkillNative(
            string id,
            string defaultVisualization,
            string defaultEnemyVisualization,
            string[] tags,
            int defaultRange,
            float defaultCd,
            float defaultMod,
            Targets availableTargets,
            Action<Scene, Actor, Tile, Skill> action)
        {
            skillNatives.Add(id, new SkillNative(id, defaultVisualization, defaultEnemyVisualization, InternTags(tags), defaultRange, defaultCd, defaultMod, availableTargets, action));
        }

        public void AddTileNative(
            string id,
            string[] tags,
            bool flat,
            int defaultHeight,
            bool unbearable,
            float defaultMod,
            Action<Scene, Tile, TileObject> onCreateAction,
            Action<Scene, Tile> onActionAction,
            Action<Scene, Tile, TileObject> onDeathAction)
        {
            tileNatives.Add(id, new TileNative(id, InternTags(tags), flat, defaultHeight, unbearable, defaultMod, onCreateAction, onActionAction, onDeathAction));
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
