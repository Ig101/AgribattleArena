using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ProjectArena.Engine.Helpers.DelegateLists;
using ProjectArena.Engine.Objects;

namespace ProjectArena.Engine.Natives
{
    public class TileNative : TaggingNative
    {
        public bool Flat { get; }

        public int DefaultHeight { get; }

        public bool Unbearable { get; }

        public Action<ISceneParentRef, Tile, float> Action { get; }

        public Action<ISceneParentRef, Tile> OnStepAction { get; }

        public float DefaultMod { get; }

        public bool RevealedByDefault { get; }

        public TileNative(string id, string[] tags, bool flat, int defaultHeight, bool unbearable, float defaultMod, bool revealedByDefault, Action<ISceneParentRef, Tile, float> action, Action<ISceneParentRef, Tile> onStepAction)
            : base(id, tags)
        {
            this.RevealedByDefault = revealedByDefault;
            this.Flat = flat;
            this.DefaultHeight = defaultHeight;
            this.Unbearable = unbearable;
            this.Action = action;
            this.OnStepAction = onStepAction;
            this.DefaultMod = defaultMod;
        }
    }
}
