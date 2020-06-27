namespace ProjectArena.Engine.Objects.Abstract
{
    public abstract class GameObject : IdObject
    {
        private bool isAlive;

        public Player Owner { get; set; }

        public bool IsAlive
        {
            get
            {
                return isAlive;
            }

            set
            {
                bool prevoiusState = isAlive;
                isAlive = value;
                if (!isAlive && prevoiusState)
                {
                    OnDeathAction();
                }
            }
        }

        public int X { get; set; }

        public int Y { get; set; }

        public float Z { get; set; }

        public GameObject(Scene parent, Player owner, int x, int y, float z)
            : base(parent)
        {
            this.Owner = owner;
            this.isAlive = true;
            this.X = x;
            this.Y = y;
            this.Z = parent.Tiles[x][y].Height + z;
        }

        public abstract void OnDeathAction();
    }
}