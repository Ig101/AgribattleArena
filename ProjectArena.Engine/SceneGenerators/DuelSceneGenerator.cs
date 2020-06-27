using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectArena.Engine.ForExternalUse.Generation.ObjectInterfaces;
using ProjectArena.Engine.Helpers;
using ProjectArena.Engine.Objects;

namespace ProjectArena.Engine.SceneGenerators
{
    public class DuelSceneGenerator : ISceneGenerator, ForExternalUse.Generation.ISceneGenerator
    {
        public Scene.DefeatConditionMethod DefeatCondition
        {
            get
            {
                return DefeatConditionDuel;
            }
        }

        public Scene.WinConditionMethod WinCondition
        {
            get
            {
                return WinConditionDuel;
            }
        }

        public string Definition
        {
            get
            {
                return "DuelScene";
            }
        }

        public DuelSceneGenerator()
        {
        }

        public void GenerateNewScene(ISceneForSceneGenerator scene, IEnumerable<IPlayer> players, int seed)
        {
            var width = 28;
            var height = 16;
            Tile[][] sceneTiles = scene.SetupEmptyTileSet(width, height);
            for (int x = 0; x < sceneTiles.Length; x++)
            {
                for (int y = 0; y < sceneTiles[x].Length; y++)
                {
                    scene.CreateTile("grass", x, y, null);
                }
            }

            List<IPlayer> tempPlayers = players.ToList();
            if (tempPlayers.Count != 2)
            {
                throw new ArgumentException("Players count should be 2", "players");
            }

            var iterator = 0;
            var tempPlayersForScene = tempPlayers.Select(player =>
            {
                iterator++;
                if (player.KeyActorsGen.Count != 6)
                {
                    throw new ArgumentException("Actors count should be 6. Thrown on player " + player.Id, "players.keyActors");
                }

                return GeneratorHelper.ConvertExternalPlayerFromGeneration(scene, player, iterator);
            }).ToList();

            tempPlayersForScene[0].KeyActors.Add(
                GeneratorHelper.ConvertExternalActorFromGeneration(scene, tempPlayersForScene[0], sceneTiles[1][1], tempPlayers[0].KeyActorsGen[0], null));
            tempPlayersForScene[0].KeyActors.Add(
                GeneratorHelper.ConvertExternalActorFromGeneration(scene, tempPlayersForScene[0], sceneTiles[4][1], tempPlayers[0].KeyActorsGen[1], null));
            tempPlayersForScene[0].KeyActors.Add(
                GeneratorHelper.ConvertExternalActorFromGeneration(scene, tempPlayersForScene[0], sceneTiles[7][1], tempPlayers[0].KeyActorsGen[2], null));
            tempPlayersForScene[0].KeyActors.Add(
                GeneratorHelper.ConvertExternalActorFromGeneration(scene, tempPlayersForScene[0], sceneTiles[1][3], tempPlayers[0].KeyActorsGen[3], null));
            tempPlayersForScene[0].KeyActors.Add(
                GeneratorHelper.ConvertExternalActorFromGeneration(scene, tempPlayersForScene[0], sceneTiles[4][3], tempPlayers[0].KeyActorsGen[4], null));
            var firstPlayerMainActor = GeneratorHelper.ConvertExternalActorFromGeneration(scene, tempPlayersForScene[0], sceneTiles[1][5], tempPlayers[0].PlayerActorGen, null);
            tempPlayersForScene[0].PlayerActor = firstPlayerMainActor;
            tempPlayersForScene[0].KeyActors.Add(firstPlayerMainActor);

            tempPlayersForScene[1].KeyActors.Add(
                GeneratorHelper.ConvertExternalActorFromGeneration(scene, tempPlayersForScene[1], sceneTiles[width - 2][height - 2], tempPlayers[1].KeyActorsGen[0], null));
            tempPlayersForScene[1].KeyActors.Add(
                GeneratorHelper.ConvertExternalActorFromGeneration(scene, tempPlayersForScene[1], sceneTiles[width - 5][height - 2], tempPlayers[1].KeyActorsGen[1], null));
            tempPlayersForScene[1].KeyActors.Add(
                GeneratorHelper.ConvertExternalActorFromGeneration(scene, tempPlayersForScene[1], sceneTiles[width - 8][height - 2], tempPlayers[1].KeyActorsGen[2], null));
            tempPlayersForScene[1].KeyActors.Add(
                GeneratorHelper.ConvertExternalActorFromGeneration(scene, tempPlayersForScene[1], sceneTiles[width - 2][height - 4], tempPlayers[1].KeyActorsGen[3], null));
            tempPlayersForScene[1].KeyActors.Add(
                GeneratorHelper.ConvertExternalActorFromGeneration(scene, tempPlayersForScene[1], sceneTiles[width - 5][height - 4], tempPlayers[1].KeyActorsGen[4], null));
            var secondPlayerMainActor = GeneratorHelper.ConvertExternalActorFromGeneration(scene, tempPlayersForScene[1], sceneTiles[width - 2][height - 6], tempPlayers[1].PlayerActorGen, null);
            tempPlayersForScene[1].PlayerActor = firstPlayerMainActor;
            tempPlayersForScene[1].KeyActors.Add(firstPlayerMainActor);
        }

        public static bool DefeatConditionDuel(Scene scene, Player player)
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

        public static bool WinConditionDuel(Scene scene)
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
    }
}
