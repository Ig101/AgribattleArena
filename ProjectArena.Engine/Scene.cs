using ProjectArena.Engine.Models;
using ProjectArena.Engine.State;
using ProjectArena.Infrastructure.Models.Battle;
using ProjectArena.Infrastructure.Models.Battle.Incoming;

namespace ProjectArena.Engine
{
    public class Scene
    {
        private readonly object _m = new object();

        private ISceneStateProvider State { get; set; }

        public Scene (
            ISceneStateProvider state)
        {
            State = state;
        }

        private void MergeSynchronizer(SynchronizerDto synchronizer)
        {
        }

        public void Update(double time)
        {
            lock (_m)
            {
            }
        }

        public void PushAction(IncomingAction action)
        {
            lock (_m)
            {
            }
        }

        public void PushSynchronization(IncomingSynchronization synchronization)
        {
            lock (_m)
            {
            }
        }
    }
}