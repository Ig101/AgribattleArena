using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ProjectArena.Engine.Helpers;
using ProjectArena.Engine.Objects;

namespace ProjectArena.Engine.Natives
{
    public class ActiveDecorationNative : TaggingNative
    {
        public string DefaultVisualisation { get; }

        public TagSynergy[] DefaultArmor { get; }

        public int DefaultHealth { get; }

        public float DefaultZ { get; }

        public float DefaultMod { get; }

        public Action<Scene, ActiveDecoration, float> OnHitAction { get; set; }

        public Action<Scene, ActiveDecoration> OnDeathAction { get; set; }

        public ActiveDecorationNative(
            string id,
            string defaultVisualization,
            string[] tags,
            TagSynergy[] defaultArmor,
            int defaultHealth,
            float defaultZ,
            float defaultMod,
            Action<Scene, ActiveDecoration, float> onHitAction,
            Action<Scene, ActiveDecoration> onDeathAction)
            : base(id, tags)
        {
            this.DefaultVisualisation = defaultVisualization;
            this.DefaultArmor = defaultArmor;
            this.DefaultHealth = defaultHealth;
            this.DefaultZ = defaultZ;
            this.DefaultMod = defaultMod;
            this.OnHitAction = onHitAction;
            this.OnDeathAction = onDeathAction;
        }
    }
}
