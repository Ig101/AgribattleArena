using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ProjectArena.Engine.Objects;

namespace ProjectArena.Engine.Natives
{
    public class SpecEffectNative : TaggingNative
    {
        public string DefaultVisualisation { get; }

        public float DefaultZ { get; }

        public float? DefaultDuration { get; }

        public float DefaultMod { get; }

        public Action<ISceneParentRef, SpecEffect, float> Action { get; }

        public Action<ISceneParentRef, SpecEffect> OnDeathAction { get; }

        public SpecEffectNative(
            string id,
            string defaultVisualization,
            string[] tags,
            float defaultZ,
            float? defaultDuration,
            float defaultMod,
            Action<ISceneParentRef, SpecEffect, float> action,
            Action<ISceneParentRef, SpecEffect> onDeathAction)
            : base(id, tags)
        {
            this.DefaultVisualisation = defaultVisualization;
            this.DefaultZ = defaultZ;
            this.DefaultDuration = defaultDuration;
            this.DefaultMod = defaultMod;
            this.Action = action;
            this.OnDeathAction = onDeathAction;
        }
    }
}
