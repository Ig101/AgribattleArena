using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AgribattleArena.Engine.Helpers;
using AgribattleArena.Engine.Helpers.DelegateLists;

namespace AgribattleArena.Engine.Natives
{
    public class ActiveDecorationNative : TaggingNative
    {
        public TagSynergy[] DefaultArmor { get; }

        public int DefaultHealth { get; }

        public float DefaultZ { get; }

        public float DefaultMod { get; }

        public ActiveDecorationActions.Action Action { get; set; }

        public ActiveDecorationActions.Action OnDeathAction { get; set; }

        public ActiveDecorationNative(string id, string[] tags, TagSynergy[] defaultArmor, int defaultHealth, float defaultZ, float defaultMod, IEnumerable<string> actionNames, IEnumerable<string> onDeathActionNames)
            : this(
                id,
                tags,
                defaultArmor,
                defaultHealth,
                defaultZ,
                defaultMod,
                actionNames.Select(actionName => (ActiveDecorationActions.Action)Delegate.CreateDelegate(
                    typeof(ActiveDecorationActions.Action),
                    typeof(ActiveDecorationActions).GetMethod(actionName, BindingFlags.Public | BindingFlags.Static))),
                onDeathActionNames.Select(actionName => (ActiveDecorationActions.Action)Delegate.CreateDelegate(
                    typeof(ActiveDecorationActions.Action),
                    typeof(ActiveDecorationActions).GetMethod(actionName, BindingFlags.Public | BindingFlags.Static))))
        {
        }

        public ActiveDecorationNative(
            string id,
            string[] tags,
            TagSynergy[] defaultArmor,
            int defaultHealth,
            float defaultZ,
            float defaultMod,
            IEnumerable<ActiveDecorationActions.Action> actions,
            IEnumerable<ActiveDecorationActions.Action> onDeathActions)
            : base(id, tags)
        {
            this.DefaultArmor = defaultArmor;
            this.DefaultHealth = defaultHealth;
            this.DefaultZ = defaultZ;
            this.DefaultMod = defaultMod;
            this.Action = null;
            foreach (ActiveDecorationActions.Action action in actions)
            {
                this.Action += action;
            }

            this.OnDeathAction = null;
            foreach (ActiveDecorationActions.Action action in onDeathActions)
            {
                this.OnDeathAction += action;
            }
        }
    }
}
