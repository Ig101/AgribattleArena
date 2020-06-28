using ProjectArena.Engine.ForExternalUse.Synchronization;

namespace ProjectArena.Engine.Synchronizers
{
    public class MoveInfo : IMoveInfo
    {
        public int ActorId { get; }

        public int TargetX { get; }

        public int TargetY { get; }

        public MoveInfo(int actorId, int targetX, int targetY)
        {
            this.ActorId = actorId;
            this.TargetX = targetX;
            this.TargetY = targetY;
        }
    }
}