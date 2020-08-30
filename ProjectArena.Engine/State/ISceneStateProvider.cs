namespace ProjectArena.Engine.State
{
    public interface ISceneStateProvider
    {
        SceneState RetrieveState();

        void PushNewState(SceneState state);
    }
}