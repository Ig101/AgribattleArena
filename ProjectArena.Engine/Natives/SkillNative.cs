using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ProjectArena.Engine.Helpers;
using ProjectArena.Engine.Helpers.DelegateLists;
using ProjectArena.Engine.Objects;
using ProjectArena.Engine.Objects.Immaterial;

namespace ProjectArena.Engine.Natives
{
    public class SkillNative : TaggingNative
    {
        public int DefaultRange { get; }

        public int DefaultCost { get; }

        public float DefaultCd { get; }

        public float DefaultMod { get; }

        public Targets AvailableTargets { get; }

        public bool OnlyVisibleTargets { get; }

        public Action<ISceneParentRef, IActorParentRef, Tile, Skill> Action { get; }

        public SkillNative(string id, string[] tags, int defaultRange, int defaultCost, float defaultCd, float defaultMod, Targets availableTargets, bool onlyVisibleTargets, Action<ISceneParentRef, IActorParentRef, Tile, Skill> action)
            : base(id, tags)
        {
            this.DefaultRange = defaultRange;
            this.DefaultCost = defaultCost;
            this.DefaultCd = defaultCd;
            this.DefaultMod = defaultMod;
            this.AvailableTargets = availableTargets;
            this.OnlyVisibleTargets = onlyVisibleTargets;
            this.Action = action;
        }
    }
}
