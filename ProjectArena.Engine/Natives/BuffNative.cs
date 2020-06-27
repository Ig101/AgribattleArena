using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ProjectArena.Engine.Objects;
using ProjectArena.Engine.Objects.Immaterial.Buffs;

namespace ProjectArena.Engine.Natives
{
    public class BuffNative : TaggingNative
    {
        public bool Eternal { get; }

        public int Repeatable { get; }

        public bool SummarizeLength { get; }

        public Action<BuffManager, Buff> Applier { get; }

        public Action<Scene, Actor, Buff> OnPurgeAction { get; }

        public float? DefaultDuration { get; }

        public float DefaultMod { get; }

        public BuffNative(
            string id,
            string[] tags,
            bool eternal,
            int repeatable,
            bool summarizeLength,
            float? defaultDuration,
            float defaultMod,
            Action<BuffManager, Buff> applier,
            Action<Scene, Actor, Buff> onPurgeAction)
            : base(id, tags)
        {
            this.Eternal = eternal;
            this.Repeatable = repeatable < 1 ? 1 : repeatable;
            this.SummarizeLength = summarizeLength;
            this.DefaultDuration = defaultDuration;
            this.DefaultMod = defaultMod;
            this.Applier = applier;
            this.OnPurgeAction = onPurgeAction;
        }
    }
}
