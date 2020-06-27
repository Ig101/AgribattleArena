using System;
using System.Collections.Generic;
using ProjectArena.Engine.ForExternalUse.Synchronization;

namespace ProjectArena.Engine.ForExternalUse
{
    public interface IScene
    {
        Guid Id { get; }

        float PassedTime { get; }

        int Version { get; }

        bool IsActive { get; }

        IEnumerable<IPlayerShort> ShortPlayers { get; }

        IEnumerable<int> GetPlayerActors(string playerId);

        IEnumerable<int> GetUserActors(string userId);

        ISynchronizer GetFullSynchronizationData();

        void Update();

        void ActorMove(string playerId, int targetX, int targetY);

        void ActorCast(string playerId, int skillId, int targetX, int targetY);

        void ActorAttack(string playerId, int targetX, int targetY);

        void ActorOrder(int actorId, int skillId, int targetX, int targetY);

        bool LeaveScene(string userId);
    }
}
