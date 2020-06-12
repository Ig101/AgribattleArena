using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ProjectArena.Engine.Helpers;
using ProjectArena.Engine.Helpers.DelegateLists;
using ProjectArena.Engine.Objects;

namespace ProjectArena.Engine.Natives
{
    public class ActiveDecorationNative : TaggingNative
    {
        public TagSynergy[] DefaultArmor { get; }

        public int DefaultHealth { get; }

        public float DefaultZ { get; }

        public float DefaultMod { get; }

        public Action<ISceneParentRef, ActiveDecoration> Action { get; set; }

        public Action<ISceneParentRef, ActiveDecoration> OnDeathAction { get; set; }

        public ActiveDecorationNative(
            string id,
            string[] tags,
            TagSynergy[] defaultArmor,
            int defaultHealth,
            float defaultZ,
            float defaultMod,
            Action<ISceneParentRef, ActiveDecoration> action,
            Action<ISceneParentRef, ActiveDecoration> onDeathAction)
            : base(id, tags)
        {
            this.DefaultArmor = defaultArmor;
            this.DefaultHealth = defaultHealth;
            this.DefaultZ = defaultZ;
            this.DefaultMod = defaultMod;
            this.Action = action;
            this.OnDeathAction = onDeathAction;
        }
    }
}
