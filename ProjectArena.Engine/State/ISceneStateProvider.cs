using System;

namespace ProjectArena.Engine.State
{
    public interface ISceneStateProvider
    {
        Guid GetSceneId();

        SceneState RetrieveState();

        void PushNewState(SceneState state);
    }
}