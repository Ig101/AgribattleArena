using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ProjectArena.Engine.Helpers;
using ProjectArena.Engine.Objects;
using ProjectArena.Engine.Objects.Immaterial;

namespace ProjectArena.Engine.Natives
{
    public class SkillNative : TaggingNative
    {
        public string DefaultVisualization { get; }

        public string DefaultEnemyVisualization { get; }

        public int DefaultRange { get; }

        public float DefaultCd { get; }

        public float DefaultMod { get; }

        public Targets AvailableTargets { get; }

        public Action<Scene, Actor, Tile, Skill> Action { get; }

        public SkillNative(
            string id,
            string defaultVisualization,
            string defaultEnemyVisualization,
            string[] tags,
            int defaultRange,
            float defaultCd,
            float defaultMod,
            Targets availableTargets,
            Action<Scene, Actor, Tile, Skill> action)
            : base(id, tags)
        {
            this.DefaultVisualization = defaultVisualization;
            this.DefaultEnemyVisualization = defaultEnemyVisualization;
            this.DefaultRange = defaultRange;
            this.DefaultCd = defaultCd;
            this.DefaultMod = defaultMod;
            this.AvailableTargets = availableTargets;
            this.Action = action;
        }
    }
}
