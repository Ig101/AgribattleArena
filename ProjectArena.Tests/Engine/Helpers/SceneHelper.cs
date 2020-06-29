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
            IVarManager varManager = EngineHelper.CreateVarManager(80, 8, 5, 0.1f, 0.1f, 0.1f, 0);

            return varManager;
        }

        public static IEnumerable<IPlayer> CreatePlayers(IEnumerable<IActor> firstActors, IEnumerable<IActor> secondActors)
        {
            List<IPlayer> players = new List<IPlayer>
            {
                EngineHelper.CreatePlayerForGeneration("1", "1", null, firstActors),
                EngineHelper.CreatePlayerForGeneration("2", "2", null, secondActors)
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
                Guid.NewGuid(),
                CreatePlayers(firstPlayer, secondPlayer),
                EngineHelper.CreateTestSceneGenerator(tileSet, winConditions),
                nativeManager,
                CreateVarManagerWithDefaultVars(),
                0,
                0,
                eventHandler);

            return scene;
        }

        public static int GetOrderByGuid(Guid? guid)
        {
            if (guid == Guid.Parse("10505fc7-3cdf-4c2b-bf36-4c934673a080"))
            {
                return 1;
            }

            if (guid == Guid.Parse("d5bd9080-cd18-475d-9a30-1a1361f99e7e"))
            {
                return 2;
            }

            return 0;
        }
    }
}
