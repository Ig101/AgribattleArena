using ProjectArena.Engine.Natives;
using ProjectArena.Engine.Objects.Abstract;

namespace ProjectArena.Engine.Objects
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

        public bool Revealed { get; set; }

        public Tile(ISceneParentRef parent, int x, int y, TileNative native, int? height)
        {
            this.Owner = null;
            this.Parent = parent;
            this.X = x;
            this.Y = y;
            this.Height = height ?? native.DefaultHeight;
            this.Native = native;
            this.Affected = true;
            this.Revealed = native.RevealedByDefault;
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
                if (!Revealed)
                {
                    Affected = true;
                    Revealed = true;
                }

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

            if (this.TempObject is Actor actor)
            {
                actor.BuffManager.RemoveTileBuffs();
            }

            TempObject = null;
            return true;
        }
    }
}
