using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectArena.Engine.ForExternalUse.Generation.ObjectInterfaces;
using ProjectArena.Engine.Helpers;
using ProjectArena.Engine.Objects;
using ProjectArena.Engine.SceneGenerators.Scenes;

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
            var positions = DefaultSceneForDuel.FillScene(scene, out Tile[][] sceneTiles);

            List<IPlayer> tempPlayers = players.ToList();
            if (tempPlayers.Count != 2)
            {
                throw new ArgumentException("Players count should be 2", "players");
            }

            var iterator = 0;
            var tempPlayersForScene = tempPlayers.Select(player =>
            {
                iterator++;
                if (player.KeyActorsGen.Count != 3)
                {
                    throw new ArgumentException("Actors count should be 3. Thrown on player " + player.Id, "players.keyActors");
                }

                return GeneratorHelper.ConvertExternalPlayerFromGeneration(scene, player, iterator);
            }).ToList();

            tempPlayersForScene[0].KeyActors.Add(
                GeneratorHelper.ConvertExternalActorFromGeneration(scene, tempPlayersForScene[0], sceneTiles[positions[0].x][positions[0].y], tempPlayers[0].KeyActorsGen[0], null));
            tempPlayersForScene[0].KeyActors.Add(
                GeneratorHelper.ConvertExternalActorFromGeneration(scene, tempPlayersForScene[0], sceneTiles[positions[1].x][positions[1].y], tempPlayers[0].KeyActorsGen[1], null));
            tempPlayersForScene[0].KeyActors.Add(
                GeneratorHelper.ConvertExternalActorFromGeneration(scene, tempPlayersForScene[0], sceneTiles[positions[2].x][positions[2].y], tempPlayers[0].KeyActorsGen[2], null));

            tempPlayersForScene[1].KeyActors.Add(
                GeneratorHelper.ConvertExternalActorFromGeneration(scene, tempPlayersForScene[1], sceneTiles[positions[3].x][positions[3].y], tempPlayers[1].KeyActorsGen[0], null));
            tempPlayersForScene[1].KeyActors.Add(
                GeneratorHelper.ConvertExternalActorFromGeneration(scene, tempPlayersForScene[1], sceneTiles[positions[4].x][positions[4].y], tempPlayers[1].KeyActorsGen[1], null));
            tempPlayersForScene[1].KeyActors.Add(
                GeneratorHelper.ConvertExternalActorFromGeneration(scene, tempPlayersForScene[1], sceneTiles[positions[5].x][positions[5].y], tempPlayers[1].KeyActorsGen[2], null));
        }

        public static bool DefeatConditionDuel(ISceneParentRef scene, IPlayerParentRef player)
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

        public static bool WinConditionDuel(ISceneParentRef scene)
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
