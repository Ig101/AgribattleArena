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
            int actionPointsIncome,
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
                actionPointsIncome,
                startBuffs);
        }

        public static IPlayer CreatePlayerForGeneration(string id, int? team, IEnumerable<IActor> keyActors)
        {
            return new Synchronizers.SynchronizationObjects.Player(id, team, keyActors.ToList());
        }

        public static IVarManager CreateVarManager(
            float turnTimeLimit,
            float turnTimeLimitAfterSkip,
            int skippedTurnsLimit,
            int maxActionPoints,
            int constitutionMod,
            float willpowerMod,
            float strengthMod,
            float speedMod)
        {
            return new VarManagers.VarManager(
                turnTimeLimit,
                turnTimeLimitAfterSkip,
                skippedTurnsLimit,
                maxActionPoints,
                constitutionMod,
                willpowerMod,
                strengthMod,
                speedMod);
        }

        public static INativeManager CreateNativeManager()
        {
            return new NativeManagers.NativeManager();
        }

        public static IScene CreateNewScene(
            long id,
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
