namespace AgribattleArena.Engine.ForExternalUse.Synchronization
{
    public interface ISyncEventArgs
    {
        Helpers.Action Action { get; }

        int Version { get; }

        IScene Scene { get; }

        ISynchronizer SyncInfo { get; }

        int? ActorId { get; }

        int? SkillActionId { get; }

        int? TargetX { get; }

        int? TargetY { get; }
    }
}
