namespace ProjectArena.Engine.State
{
    public interface ISceneStateProvider
    {
        string GetSceneId();

        SceneState RetrieveState();

        void PushNewState(SceneState state);
    }
}