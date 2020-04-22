namespace AgribattleArena.Engine.ForExternalUse.Synchronization.ObjectInterfaces
{
    public interface ISpecEffect
    {
        int Id { get; }

        string OwnerId { get; }

        bool IsAlive { get; }

        int X { get; }

        int Y { get; }

        float Z { get; }

        float? Duration { get; }

        float Mod { get; }

        string NativeId { get; }
    }
}
