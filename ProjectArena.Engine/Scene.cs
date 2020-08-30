using ProjectArena.Engine.State;
using ProjectArena.Infrastructure.Models.Battle;
using ProjectArena.Infrastructure.Models.Battle.Incoming;

namespace ProjectArena.Engine
{
    public class Scene
    {
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
        }
    }
}