namespace ProjectArena.Engine.ForExternalUse.Synchronization
{
    public interface IMoveInfo
    {
        int ActorId { get; }

        int TargetX { get; }

        int TargetY { get; }
    }
}