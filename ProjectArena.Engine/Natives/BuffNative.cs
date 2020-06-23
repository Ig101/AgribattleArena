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

        public bool OnTile { get; }

        public Action<ISceneParentRef, IActorParentRef, Buff, float> Action { get; }

        public Action<IBuffManagerParentRef, Buff> Applier { get; }

        public Action<ISceneParentRef, IActorParentRef, Buff> OnPurgeAction { get; }

        public float? DefaultDuration { get; }

        public float DefaultMod { get; }

        public BuffNative(
            string id,
            string[] tags,
            bool eternal,
            int repeatable,
            bool summarizeLength,
            bool onTile,
            float? defaultDuration,
            float defaultMod,
            Action<ISceneParentRef, IActorParentRef, Buff, float> action,
            Action<IBuffManagerParentRef, Buff> applier,
            Action<ISceneParentRef, IActorParentRef, Buff> onPurgeAction)
            : base(id, tags)
        {
            this.Eternal = eternal;
            this.Repeatable = repeatable < 1 ? 1 : repeatable;
            this.SummarizeLength = summarizeLength;
            this.OnTile = onTile;
            this.DefaultDuration = defaultDuration;
            this.DefaultMod = defaultMod;
            this.Action = action;
            this.Applier = applier;
            this.OnPurgeAction = onPurgeAction;
        }
    }
}
