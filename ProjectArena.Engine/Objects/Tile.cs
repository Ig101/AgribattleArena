using ProjectArena.Engine.Natives;
using ProjectArena.Engine.Objects.Abstract;

namespace ProjectArena.Engine.Objects
{
    public class Tile
    {
        public Player Owner { get; set; }

        public bool Affected { get; set; }

        public Scene Parent { get; }

        public TileObject TempObject { get; private set; }

        public float Height { get; set; }

        public int X { get; }

        public int Y { get; }

        public TileNative Native { get; set; }

        public Tile(Scene parent, int x, int y, TileNative native, int? height)
        {
            this.Owner = null;
            this.Parent = parent;
            this.X = x;
            this.Y = y;
            this.Height = height ?? native.DefaultHeight;
            this.Native = native;
            this.Affected = true;
        }

        public bool ChangeTempObject(TileObject tileObject)
        {
            if (TempObject != null)
            {
                return false;
            }

            TempObject = tileObject;
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
