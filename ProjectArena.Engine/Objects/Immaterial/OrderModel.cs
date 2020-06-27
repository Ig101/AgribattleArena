using ProjectArena.Engine.Objects.Abstract;

namespace ProjectArena.Engine.Objects.Immaterial
{
    public class OrderModel
    {
        public bool Intended { get; set; }

        public Skill Skill { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public TileObject Target { get; set; }

        public OrderModel(bool intended, Skill skill, int x, int y)
        {
            Intended = intended;
            Skill = skill;
            X = x;
            Y = y;
        }

        public OrderModel(bool intended, Skill skill, TileObject target)
            : this(intended, skill, target.X, target.Y)
        {
            Target = target;
        }
    }
}