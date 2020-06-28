using System.Collections.Generic;
using ProjectArena.Engine.Objects;

namespace ProjectArena.Engine.SceneGenerators.Scenes
{
    public static class DefaultSceneForDuel
    {
        public static (int x, int y)[] FillScene(ISceneForSceneGenerator scene, out Tile[][] sceneTiles)
        {
            var width = 28;
            var height = 16;
            sceneTiles = scene.SetupEmptyTileSet(width, height);
            for (int x = 9; x <= 18; x++)
            {
                for (int y = 5; y <= 9; y++)
                {
                    if (sceneTiles[x][y] == null)
                    {
                        scene.CreateTile("grass", x, y, 25);
                    }
                }
            }

            scene.CreateTile("grass", 13, 4, 17);
            scene.CreateTile("grass", 14, 4, 17);
            scene.CreateTile("grass", 13, 3, 8);
            scene.CreateTile("grass", 14, 3, 8);
            scene.CreateTile("grass", 12, 4, 17);
            scene.CreateTile("grass", 15, 4, 17);
            scene.CreateTile("grass", 12, 3, 8);
            scene.CreateTile("grass", 15, 3, 8);
            scene.CreateTile("grass", 11, 3, 8);
            scene.CreateTile("grass", 16, 3, 8);
            scene.CreateTile("grass", 11, 4, 8);
            scene.CreateTile("grass", 16, 4, 8);

            scene.CreateTile("bush", 0, 0, 16);
            scene.CreateTile("bush", 0, 1, 16);
            scene.CreateTile("bush", 0, 2, 16);
            scene.CreateTile("bush", 0, 3, 16);
            scene.CreateTile("bush", 0, 4, 16);
            scene.CreateTile("bush", 0, 5, 9);
            scene.CreateTile("bush", 0, 11, 9);
            scene.CreateTile("bush", 0, 12, 16);
            scene.CreateTile("bush", 0, 13, 16);
            scene.CreateTile("bush", 0, 14, 16);
            scene.CreateTile("bush", 0, 15, 16);

            scene.CreateTile("bush", 1, 0, 16);
            scene.CreateTile("bush", 1, 1, 16);
            scene.CreateTile("bush", 1, 2, 16);
            scene.CreateTile("bush", 1, 3, 16);
            scene.CreateTile("bush", 1, 4, 16);
            scene.CreateTile("bush", 1, 5, 9);
            scene.CreateTile("bush", 1, 11, 9);
            scene.CreateTile("bush", 1, 12, 16);
            scene.CreateTile("bush", 1, 13, 16);
            scene.CreateTile("bush", 1, 14, 16);
            scene.CreateTile("bush", 1, 15, 16);

            scene.CreateTile("bush", 2, 0, 16);
            scene.CreateTile("bush", 2, 1, 16);
            scene.CreateTile("bush", 2, 2, 16);
            scene.CreateTile("bush", 2, 3, 16);
            scene.CreateTile("bush", 2, 4, 9);
            scene.CreateTile("bush", 2, 12, 9);
            scene.CreateTile("bush", 2, 13, 9);
            scene.CreateTile("bush", 2, 14, 16);
            scene.CreateTile("bush", 2, 15, 16);

            scene.CreateTile("bush", 3, 0, 16);
            scene.CreateTile("bush", 3, 1, 16);
            scene.CreateTile("bush", 3, 2, 9);
            scene.CreateTile("bush", 3, 3, 9);
            scene.CreateTile("bush", 3, 14, 9);
            scene.CreateTile("bush", 3, 15, 9);

            scene.CreateTile("bush", 4, 0, 9);
            scene.CreateTile("bush", 4, 1, 9);

            scene.CreateTile("bush", 27, 0, 16);
            scene.CreateTile("bush", 27, 1, 16);
            scene.CreateTile("bush", 27, 2, 16);
            scene.CreateTile("bush", 27, 3, 16);
            scene.CreateTile("bush", 27, 4, 16);
            scene.CreateTile("bush", 27, 5, 9);
            scene.CreateTile("bush", 27, 11, 9);
            scene.CreateTile("bush", 27, 12, 16);
            scene.CreateTile("bush", 27, 13, 16);
            scene.CreateTile("bush", 27, 14, 16);
            scene.CreateTile("bush", 27, 15, 16);

            scene.CreateTile("bush", 26, 0, 16);
            scene.CreateTile("bush", 26, 1, 16);
            scene.CreateTile("bush", 26, 2, 16);
            scene.CreateTile("bush", 26, 3, 16);
            scene.CreateTile("bush", 26, 4, 16);
            scene.CreateTile("bush", 26, 5, 9);
            scene.CreateTile("bush", 26, 11, 9);
            scene.CreateTile("bush", 26, 12, 16);
            scene.CreateTile("bush", 26, 13, 16);
            scene.CreateTile("bush", 26, 14, 16);
            scene.CreateTile("bush", 26, 15, 16);

            scene.CreateTile("bush", 25, 0, 16);
            scene.CreateTile("bush", 25, 1, 16);
            scene.CreateTile("bush", 25, 2, 16);
            scene.CreateTile("bush", 25, 3, 16);
            scene.CreateTile("bush", 25, 4, 9);
            scene.CreateTile("bush", 25, 12, 9);
            scene.CreateTile("bush", 25, 13, 9);
            scene.CreateTile("bush", 25, 14, 16);
            scene.CreateTile("bush", 25, 15, 16);

            scene.CreateTile("bush", 24, 0, 16);
            scene.CreateTile("bush", 24, 1, 16);
            scene.CreateTile("bush", 24, 2, 9);
            scene.CreateTile("bush", 24, 3, 9);
            scene.CreateTile("bush", 24, 14, 9);
            scene.CreateTile("bush", 24, 15, 9);

            scene.CreateTile("bush", 23, 0, 9);
            scene.CreateTile("bush", 23, 1, 9);

            for (int x = 0; x < sceneTiles.Length; x++)
            {
                for (int y = 0; y < sceneTiles[x].Length; y++)
                {
                    if (sceneTiles[x][y] == null)
                    {
                        scene.CreateTile("grass", x, y, null);
                    }
                }
            }

            // Trees
            scene.CreateDecoration(null, "tree", sceneTiles[5][5], null, null, null, null, null);
            scene.CreateDecoration(null, "tree", sceneTiles[5][15], null, null, null, null, null);

            scene.CreateDecoration(null, "tree", sceneTiles[6][0], null, null, null, null, null);

            scene.CreateDecoration(null, "tree", sceneTiles[7][3], null, null, null, null, null);
            scene.CreateDecoration(null, "tree", sceneTiles[7][6], null, null, null, null, null);

            scene.CreateDecoration(null, "tree", sceneTiles[8][1], null, null, null, null, null);

            scene.CreateDecoration(null, "tree", sceneTiles[10][3], null, null, null, null, null);

            scene.CreateDecoration(null, "tree", sceneTiles[11][0], null, null, null, null, null);

            scene.CreateDecoration(null, "tree", sceneTiles[11][4], null, null, null, null, null);

            scene.CreateDecoration(null, "tree", sceneTiles[22][5], null, null, null, null, null);
            scene.CreateDecoration(null, "tree", sceneTiles[22][15], null, null, null, null, null);

            scene.CreateDecoration(null, "tree", sceneTiles[21][0], null, null, null, null, null);

            scene.CreateDecoration(null, "tree", sceneTiles[20][3], null, null, null, null, null);
            scene.CreateDecoration(null, "tree", sceneTiles[20][6], null, null, null, null, null);

            scene.CreateDecoration(null, "tree", sceneTiles[19][1], null, null, null, null, null);

            scene.CreateDecoration(null, "tree", sceneTiles[17][3], null, null, null, null, null);

            scene.CreateDecoration(null, "tree", sceneTiles[16][0], null, null, null, null, null);

            scene.CreateDecoration(null, "tree", sceneTiles[16][4], null, null, null, null, null);

            return new (int x, int y)[]
            {
                (3, 8),
                (1, 7),
                (1, 9),
                (24, 8),
                (26, 7),
                (26, 9)
            };
        }
    }
}