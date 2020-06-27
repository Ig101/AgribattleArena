using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ProjectArena.Engine.Objects;
using ProjectArena.Engine.Objects.Abstract;

namespace ProjectArena.Engine.Natives
{
    public class TileNative : TaggingNative
    {
        public bool Flat { get; }

        public int DefaultHeight { get; }

        public bool Unbearable { get; }

        public Action<Scene, Tile, TileObject> OnCreateAction { get; }

        public Action<Scene, Tile, TileObject> OnDeathAction { get; }

        public Action<Scene, Tile> OnActionAction { get; }

        public float DefaultMod { get; }

        public TileNative(
            string id,
            string[] tags,
            bool flat,
            int defaultHeight,
            bool unbearable,
            float defaultMod,
            Action<Scene, Tile, TileObject> onCreateAction,
            Action<Scene, Tile> onActionAction,
            Action<Scene, Tile, TileObject> onDeathAction)
            : base(id, tags)
        {
            this.Flat = flat;
            this.DefaultHeight = defaultHeight;
            this.Unbearable = unbearable;
            this.OnActionAction = onActionAction;
            this.OnCreateAction = onCreateAction;
            this.OnDeathAction = onDeathAction;
            this.DefaultMod = defaultMod;
        }
    }
}
