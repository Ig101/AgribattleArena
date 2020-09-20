using System;

namespace ProjectArena.Engine.State
{
    public class SceneStateProvider : ISceneStateProvider
    {
        private SceneState _state;

        public SceneStateProvider(SceneState state)
        {
            _state = state;
        }

        public Guid GetSceneId()
        {
            return _state.Id;
        }

        public void PushNewState(SceneState state)
        {
            _state = state;
        }

        public SceneState RetrieveState()
        {
            return _state.Clone();
        }
    }
}