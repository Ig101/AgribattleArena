namespace ProjectArena.Engine.State
{
    public class SceneStateProvider : ISceneStateProvider
    {
        private SceneState _state;

        public SceneStateProvider(SceneState state)
        {
            _state = state;
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