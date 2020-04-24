using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ProjectArena.Engine.Helpers.DelegateLists;

namespace ProjectArena.Engine.Natives
{
    public class SpecEffectNative : TaggingNative
    {
        public float DefaultZ { get; }

        public float? DefaultDuration { get; }

        public float DefaultMod { get; }

        public SpecEffectActions.Action Action { get; }

        public SpecEffectActions.OnDeathAction OnDeathAction { get; }

        public SpecEffectNative(string id, string[] tags, float defaultZ, float? defaultDuration, float defaultMod, IEnumerable<string> actionNames, IEnumerable<string> onDeathActionNames)
            : this(
                id,
                tags,
                defaultZ,
                defaultDuration,
                defaultMod,
                actionNames.Select(actionName => (SpecEffectActions.Action)Delegate.CreateDelegate(
                    typeof(SpecEffectActions.Action),
                    typeof(SpecEffectActions).GetMethod(actionName, BindingFlags.Public | BindingFlags.Static))),
                onDeathActionNames.Select(actionName => (SpecEffectActions.OnDeathAction)Delegate.CreateDelegate(
                    typeof(SpecEffectActions.OnDeathAction),
                    typeof(SpecEffectActions).GetMethod(actionName, BindingFlags.Public | BindingFlags.Static))))
        {
        }

        public SpecEffectNative(
            string id,
            string[] tags,
            float defaultZ,
            float? defaultDuration,
            float defaultMod,
            IEnumerable<SpecEffectActions.Action> actions,
            IEnumerable<SpecEffectActions.OnDeathAction> onDeathActions)
            : base(id, tags)
        {
            this.DefaultZ = defaultZ;
            this.DefaultDuration = defaultDuration;
            this.DefaultMod = defaultMod;
            this.Action = null;
            foreach (SpecEffectActions.Action action in actions)
            {
                this.Action += action;
            }

            this.OnDeathAction = null;
            foreach (SpecEffectActions.OnDeathAction action in onDeathActions)
            {
                this.OnDeathAction += action;
            }
        }
    }
}
