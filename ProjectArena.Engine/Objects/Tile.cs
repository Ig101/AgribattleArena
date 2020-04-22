using AgribattleArena.Engine.Natives;
using AgribattleArena.Engine.Objects.Abstract;

namespace AgribattleArena.Engine.Objects
{
    public class Tile : ITileParentRef
    {
        public IPlayerParentRef Owner { get; set; }

        public bool Affected { get; set; }

        public ISceneParentRef Parent { get; }

        public TileObject TempObject { get; private set; }

        public float Height { get; set; }

        public int X { get; }

        public int Y { get; }

        public TileNative Native { get; set; }

        public Tile(ISceneParentRef parent, int x, int y, TileNative native, int? height)
        {
            this.Owner = null;
            this.Parent = parent;
            this.X = x;
            this.Y = y;
            this.Height = height ?? native.DefaultHeight;
            this.Native = native;
            this.Affected = true;
        }

        public void Update(float time)
        {
            Native.Action?.Invoke(Parent, this, time);
        }

        public bool ChangeTempObject(TileObject tileObject, bool trigger)
        {
            if (TempObject != null)
            {
                return false;
            }

            Affected = true;
            TempObject = tileObject;
            if (trigger)
            {
                Native.OnStepAction?.Invoke(Parent, this);
            }

            return true;
        }

        public bool RemoveTempObject()
        {
            if (TempObject == null)
            {
                return false;
            }

            TempObject = null;
            return true;
        }
    }
}
