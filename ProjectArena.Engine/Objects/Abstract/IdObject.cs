namespace AgribattleArena.Engine.Objects.Abstract
{
    public abstract class IdObject
    {
        public ISceneParentRef Parent { get; }

        public int Id { get; }

        public IdObject(ISceneParentRef parent)
        {
            this.Parent = parent;
            this.Id = parent.GetNextId();
        }
    }
}
