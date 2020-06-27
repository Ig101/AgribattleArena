namespace ProjectArena.Engine.Objects.Abstract
{
    public abstract class IdObject
    {
        public Scene Parent { get; }

        public int Id { get; }

        public IdObject(Scene parent)
        {
            this.Parent = parent;
            this.Id = parent.GetNextId();
        }
    }
}
