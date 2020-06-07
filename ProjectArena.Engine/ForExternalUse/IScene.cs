using System;
using System.Collections.Generic;
using ProjectArena.Engine.ForExternalUse.Synchronization;

namespace ProjectArena.Engine.ForExternalUse
{
    public interface IScene
    {
        Guid Id { get; }

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
