namespace ProjectArena.Engine.ForExternalUse.Synchronization
{
    public interface IMoveEventArgs
    {
        int? ActorId { get; }

        int? TargetX { get; }

        int? TargetY { get; }
    }
}