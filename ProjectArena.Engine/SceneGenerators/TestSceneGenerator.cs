using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectArena.Engine.ForExternalUse.Generation.ObjectInterfaces;
using ProjectArena.Engine.Helpers;
using ProjectArena.Engine.Natives;
using ProjectArena.Engine.Objects;
using ProjectArena.Engine.Objects.Immaterial;

namespace ProjectArena.Engine.SceneGenerators
{
    public class TestSceneGenerator : ISceneGenerator, ForExternalUse.Generation.ISceneGenerator
    {
        public Scene.DefeatConditionMethod DefeatCondition { get; set; }

        public Scene.WinConditionMethod WinCondition { get; set; }

        public string Definition
        {
            get
            {
                return "TestScene";
            }
        }

        private readonly string[,] tileSet;

        public TestSceneGenerator(string[,] tileSet, bool winConditions)
        {
            if (winConditions)
            {
                WinCondition = WinConditionTest;
                DefeatCondition = DefeatConditionTest;
            }
            else
            {
                WinCondition = WinConditionDummy;
                DefeatCondition = DefeatConditionDummy;
            }

            if (tileSet.GetLength(0) != 20)
            {
                throw new ArgumentException("Number of rows doesn't equal to 20");
            }

            if (tileSet.GetLength(1) != 20)
            {
                throw new ArgumentException("Number of columns doesn't equal to 20");
            }

            this.tileSet = tileSet;
        }

        public void GenerateNewScene(ISceneForSceneGenerator scene, IEnumerable<IPlayer> players, int seed)
        {
            Tile[][] sceneTiles = scene.SetupEmptyTileSet(20, 20);
            for (int x = 0; x < sceneTiles.Length; x++)
            {
                for (int y = 0; y < sceneTiles[x].Length; y++)
                {
                    scene.CreateTile(tileSet[x, y], x, y, null);
                }
            }

            List<IPlayer> tempPlayers = players.ToList();
            if (tempPlayers.Count != 2)
            {
                throw new ArgumentException("Players count should be 2", "players");
            }

            for (int i = 0; i < 2; i++)
            {
                if (tempPlayers[i].KeyActorsGen.Count > 5)
                {
                throw new ArgumentException("Actors count should be less than 5. Thrown on player " + tempPlayers[i].Id, "players.keyActors");
                }

                Player tempScenePlayer = GeneratorHelper.ConvertExternalPlayerFromGeneration(scene, tempPlayers[i], i);
                for (int j = 0; j < tempPlayers[i].KeyActorsGen.Count; j++)
                {
                    tempScenePlayer.KeyActors.Add(GeneratorHelper.ConvertExternalActorFromGeneration(scene, tempScenePlayer, sceneTiles[1 + (i * 17)][2 + (j * 2)], tempPlayers[i].KeyActorsGen[j], null));
                }
            }
        }

        public static bool DefeatConditionTest(Scene scene, Player player)
        {
            foreach (Actor actor in player.KeyActors)
            {
                if (actor.IsAlive)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool WinConditionTest(Scene scene)
        {
            int countOfRemainedPlayers = 0;
            foreach (Player player in scene.Players)
            {
                if (player.Status == PlayerStatus.Playing)
                {
                    countOfRemainedPlayers++;
                }
            }

            return countOfRemainedPlayers <= 1;
        }

        public static bool DefeatConditionDummy(Scene scene, Player player)
        {
            return false;
        }

        public static bool WinConditionDummy(Scene scene)
        {
            return false;
        }
    }
}
