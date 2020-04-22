using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AgribattleArena.Engine.Helpers.DelegateLists;

namespace AgribattleArena.Engine.Natives
{
    public class TileNative : TaggingNative
    {
        public bool Flat { get; }

        public int DefaultHeight { get; }

        public bool Unbearable { get; }

        public TileActions.Action Action { get; }

        public TileActions.OnStepAction OnStepAction { get; }

        public float DefaultMod { get; }

        public TileNative(string id, string[] tags, bool flat, int defaultHeight, bool unbearable, float defaultMod, IEnumerable<string> actionNames, IEnumerable<string> onStepActionNames)
            : this(
                id,
                tags,
                flat,
                defaultHeight,
                unbearable,
                defaultMod,
                actionNames.Select(actionName => (TileActions.Action)Delegate.CreateDelegate(
                    typeof(TileActions.Action),
                    typeof(TileActions).GetMethod(actionName, BindingFlags.Public | BindingFlags.Static))),
                onStepActionNames.Select(onStepActionName => (TileActions.OnStepAction)Delegate.CreateDelegate(
                    typeof(TileActions.OnStepAction),
                    typeof(TileActions).GetMethod(onStepActionName, BindingFlags.Public | BindingFlags.Static))))
        {
        }

        public TileNative(string id, string[] tags, bool flat, int defaultHeight, bool unbearable, float defaultMod, IEnumerable<TileActions.Action> actions, IEnumerable<TileActions.OnStepAction> onStepActions)
            : base(id, tags)
        {
            this.Flat = flat;
            this.DefaultHeight = defaultHeight;
            this.Unbearable = unbearable;
            this.Action = null;
            foreach (TileActions.Action action in actions)
            {
                this.Action += action;
            }

            this.OnStepAction = null;
            this.OnStepAction += TileActions.PurgeTileEffects;
            foreach (TileActions.OnStepAction action in onStepActions)
            {
                this.OnStepAction += action;
            }

            this.DefaultMod = defaultMod;
        }
    }
}
