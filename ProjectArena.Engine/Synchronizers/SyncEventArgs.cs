using System;
using ProjectArena.Engine.ForExternalUse;
using ProjectArena.Engine.ForExternalUse.Synchronization;

namespace ProjectArena.Engine.Synchronizers
{
    public class SyncEventArgs : EventArgs, ISyncEventArgs
    {
        public Helpers.Action Action { get; }

        public int Version { get; }

        public IScene Scene { get; }

        public ISynchronizer SyncInfo { get; }

        public int? ActorId { get; }

        public int? SkillActionId { get; }

        public int? TargetX { get; }

        public int? TargetY { get; }

        public SyncEventArgs(IScene scene, int version, Helpers.Action action, ISynchronizer syncInfo, int? id, int? actionId, int? targetX, int? targetY)
        {
            this.Version = version;
            this.Scene = scene;
            this.Action = action;
            this.SyncInfo = syncInfo;
            this.ActorId = id;
            this.SkillActionId = actionId;
            this.TargetX = targetX;
            this.TargetY = targetY;
        }
    }
}
