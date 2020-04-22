using System.Collections.Generic;
using AgribattleArena.Engine.ForExternalUse.Synchronization;

namespace AgribattleArena.Engine.ForExternalUse
{
    public interface IScene
    {
        float PassedTime { get; }

        string EnemyActorsPrefix { get; }

        int Version { get; }

        bool IsActive { get; }

        IEnumerable<IPlayerShort> ShortPlayers { get; }

        IEnumerable<int> GetPlayerActors(string playerId);

        float RemainedTurnTime { get; }

        ISynchronizer GetFullSynchronizationData();

        void UpdateTime(float time);

        bool ActorMove(int actorId, int targetX, int targetY);

        bool ActorCast(int actorId, int skillId, int targetX, int targetY);

        bool ActorAttack(int actorId, int targetX, int targetY);

        bool ActorWait(int actorId);
    }
}
