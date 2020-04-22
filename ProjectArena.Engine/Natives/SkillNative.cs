using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AgribattleArena.Engine.Helpers.DelegateLists;

namespace AgribattleArena.Engine.Natives
{
    public class SkillNative : TaggingNative
    {
        public int DefaultRange { get; }

        public int DefaultCost { get; }

        public float DefaultCd { get; }

        public float DefaultMod { get; }

        public SkillActions.Action Action { get; }

        public SkillNative(string id, string[] tags, int defaultRange, int defaultCost, float defaultCd, float defaultMod, IEnumerable<string> actionNames)
            : this(
                id,
                tags,
                defaultRange,
                defaultCost,
                defaultCd,
                defaultMod,
                actionNames.Select(actionName =>
                    (SkillActions.Action)Delegate.CreateDelegate(typeof(SkillActions.Action), typeof(SkillActions).GetMethod(actionName, BindingFlags.Public | BindingFlags.Static))))
        {
        }

        public SkillNative(string id, string[] tags, int defaultRange, int defaultCost, float defaultCd, float defaultMod, IEnumerable<SkillActions.Action> actions)
            : base(id, tags)
        {
            this.DefaultRange = defaultRange;
            this.DefaultCost = defaultCost;
            this.DefaultCd = defaultCd;
            this.DefaultMod = defaultMod;
            this.Action = null;
            foreach (SkillActions.Action action in actions)
            {
                this.Action += action;
            }
        }
    }
}
