using System;
using ProjectArena.Engine.ForExternalUse;
using ProjectArena.Engine.ForExternalUse.Synchronization;

namespace ProjectArena.Engine.Synchronizers
{
    public class MoveEventArgs : EventArgs, IMoveEventArgs
    {
        public int? ActorId { get; }

        public int? TargetX { get; }

        public int? TargetY { get; }

        public MoveEventArgs(int? actorId, int? targetX, int? targetY)
        {
            this.ActorId = actorId;
            this.TargetX = targetX;
            this.TargetY = targetY;
        }
    }
}
