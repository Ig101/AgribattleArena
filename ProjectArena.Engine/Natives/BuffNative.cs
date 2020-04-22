using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AgribattleArena.Engine.Helpers.DelegateLists;

namespace AgribattleArena.Engine.Natives
{
    public class BuffNative : TaggingNative
    {
        public bool Eternal { get; }

        public int Repeatable { get; }

        public bool SummarizeLength { get; }

        public BuffActions.Action Action { get; }

        public BuffActions.Applier Applier { get; }

        public BuffActions.OnPurgeAction OnPurgeAction { get; }

        public float? DefaultDuration { get; }

        public float DefaultMod { get; }

        public BuffNative(string id, string[] tags, bool eternal, int repeatable, bool summarizeLength, float? defaultDuration, float defaultMod, IEnumerable<string> actionNames, IEnumerable<string> applierNames, IEnumerable<string> onPurgeActionNames)
            : this(
                id,
                tags,
                eternal,
                repeatable,
                summarizeLength,
                defaultDuration,
                defaultMod,
                actionNames.Select(actionName => (BuffActions.Action)Delegate.CreateDelegate(
                    typeof(BuffActions.Action),
                    typeof(BuffActions).GetMethod(actionName, BindingFlags.Public | BindingFlags.Static))),
                applierNames.Select(applierName => (BuffActions.Applier)Delegate.CreateDelegate(
                    typeof(BuffActions.Applier),
                    typeof(BuffActions).GetMethod(applierName, BindingFlags.Public | BindingFlags.Static))),
                onPurgeActionNames.Select(actionName => (BuffActions.OnPurgeAction)Delegate.CreateDelegate(
                    typeof(BuffActions.OnPurgeAction),
                    typeof(BuffActions).GetMethod(actionName, BindingFlags.Public | BindingFlags.Static))))
        {
        }

        public BuffNative(
            string id,
            string[] tags,
            bool eternal,
            int repeatable,
            bool summarizeLength,
            float? defaultDuration,
            float defaultMod,
            IEnumerable<BuffActions.Action> actions,
            IEnumerable<BuffActions.Applier> appliers,
            IEnumerable<BuffActions.OnPurgeAction> onPurgeActions)
            : base(id, tags)
        {
            this.Eternal = eternal;
            this.Repeatable = repeatable < 1 ? 1 : repeatable;
            this.SummarizeLength = summarizeLength;
            this.DefaultDuration = defaultDuration;
            this.DefaultMod = defaultMod;
            this.Action = null;
            foreach (BuffActions.Action action in actions)
            {
                this.Action += action;
            }

            this.Applier = null;
            foreach (BuffActions.Applier applier in appliers)
            {
                this.Applier += applier;
            }

            this.OnPurgeAction = null;
            foreach (BuffActions.OnPurgeAction action in onPurgeActions)
            {
                this.OnPurgeAction += action;
            }
        }
    }
}
