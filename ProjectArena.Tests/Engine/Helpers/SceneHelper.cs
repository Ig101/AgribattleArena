using System;
using System.Collections.Generic;
using System.Text;
using ProjectArena.Engine;
using ProjectArena.Engine.ForExternalUse;
using ProjectArena.Engine.ForExternalUse.EngineHelper;
using ProjectArena.Engine.ForExternalUse.Generation;
using ProjectArena.Engine.ForExternalUse.Generation.ObjectInterfaces;
using ProjectArena.Engine.ForExternalUse.Synchronization;

namespace ProjectArena.Tests.Engine.Helpers
{
    public static class SceneHelper
    {
        public static IVarManager CreateVarManagerWithDefaultVars()
        {
            IVarManager varManager = EngineHelper.CreateVarManager(80, 20, 3, 8, 5, 0.1f, 0.1f, 0.1f);

            return varManager;
        }

        public static IEnumerable<IPlayer> CreatePlayers(IEnumerable<IActor> firstActors, IEnumerable<IActor> secondActors)
        {
            List<IPlayer> players = new List<IPlayer>
            {
                EngineHelper.CreatePlayerForGeneration("1", null, firstActors),
                EngineHelper.CreatePlayerForGeneration("2", null, secondActors)
            };
            return players;
        }

        public static Scene CreateNewScene (
            INativeManager nativeManager,
            string[,] tileSet,
            bool winConditions,
            IEnumerable<IActor> firstPlayer,
            IEnumerable<IActor> secondPlayer,
            EventHandler<ISyncEventArgs> eventHandler)
        {
            Scene scene = (Scene)EngineHelper.CreateNewScene(
                0,
                CreatePlayers(firstPlayer, secondPlayer),
                EngineHelper.CreateTestSceneGenerator(tileSet, winConditions),
                nativeManager,
                CreateVarManagerWithDefaultVars(),
                0,
                eventHandler);

            return scene;
        }
    }
}
