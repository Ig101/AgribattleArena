using System;
using System.Collections.Generic;
using System.Linq;
using ProjectArena.Engine.ForExternalUse.Generation;
using ProjectArena.Engine.ForExternalUse.Generation.ObjectInterfaces;
using ProjectArena.Engine.ForExternalUse.Synchronization;

namespace ProjectArena.Engine.ForExternalUse.EngineHelper
{
    public static class EngineHelper
    {
        public static ISceneGenerator CreateTestSceneGenerator(string[,] tileSet, bool winConditions)
        {
            return new SceneGenerators.TestSceneGenerator(tileSet, winConditions);
        }

        public static ISceneGenerator CreateDuelSceneGenerator()
        {
            return new SceneGenerators.DuelSceneGenerator();
        }

        public static IActor CreateActorForGeneration (
            Guid? externalId,
            string nativeId,
            string attackingSkillName,
            int strength,
            int willpower,
            int constitution,
            int speed,
            IEnumerable<string> skillNames,
            IEnumerable<string> startBuffs)
        {
            return new Synchronizers.SynchronizationObjects.Actor(
                externalId,
                nativeId,
                attackingSkillName,
                strength,
                willpower,
                constitution,
                speed,
                skillNames,
                startBuffs);
        }

        public static IPlayer CreatePlayerForGeneration(string id, string userId, int? team, IEnumerable<IActor> keyActors)
        {
            return new Synchronizers.SynchronizationObjects.Player(id, userId, team, keyActors.ToList());
        }

        public static IVarManager CreateVarManager(
            int constitutionMod,
            float willpowerMod,
            float strengthMod,
            float speedMod,
            float minimumInitiative)
        {
            return new VarManagers.VarManager(
                constitutionMod,
                willpowerMod,
                strengthMod,
                speedMod,
                minimumInitiative);
        }

        public static INativeManager CreateNativeManager()
        {
            return new NativeManagers.NativeManager();
        }

        public static IScene CreateNewScene(
            Guid id,
            IEnumerable<Generation.ObjectInterfaces.IPlayer> players,
            ISceneGenerator generator,
            INativeManager nativeManager,
            IVarManager varManager,
            int seed,
            EventHandler<ISyncEventArgs> eventHandler)
        {
            Scene scene = new Scene(id, players, generator, nativeManager, varManager, seed);
            scene.ReturnAction += eventHandler;
            scene.StartGame();
            return scene;
        }
    }
}
