#region

using System.Collections.Generic;
using Game.Runtime.Islands;
using Game.Runtime.Maps;
using Game.Runtime.Maps.MapObjects;
using UnityEngine;

#endregion

namespace Game.Runtime.UtilitiesContainer
{
    public static class MapUtilities
    {

        private const float TileXOffset = 1f;
        private const float TileYOffset = 0.855f;

        public static Vector3 ToWorldPosition(this Vector2Int from, float z = 0f)
        {
            return new Vector3(from.y % 2 == 0 ?
                (from.x * TileXOffset) :
                (from.x * TileXOffset + TileXOffset / 2f), z, from.y * TileYOffset);
        }

        private static bool OutOfBounds(Vector2Int position, Vector2Int mapSize)
        {
            return !(position.x >= 0 && position.x < mapSize.x && position.y >= 0 && position.y < mapSize.y);
        }


        public static Vector2Int[] GetNeighbours(Vector2Int position)
        {
            return GetNeighbours(position.y, position.x);
        }


        private static Vector2Int[] GetNeighbours(int y, int x)
        {
            bool isEvenCol = x % 2 == 0;
            bool isEvenRow = y % 2 == 0;

            if (isEvenRow)
            {
                return new[]
                {
                    new Vector2Int(x, y - 1), // 0
                    new Vector2Int(x - 1, y - x % 2), // 1
                    new Vector2Int(isEvenCol ? x + 1 : x - 1, isEvenCol ? y - x % 2 : y + 1), // 2
                    new Vector2Int(x - 1, y + 1 - x % 2), // 3
                    new Vector2Int(isEvenCol ? x - 1 : x + 1, isEvenCol ? y - 1 : y + 1 - x % 2), // 4
                    new Vector2Int(x, y + 1) // 5
                };
            }

            return new[]
            {
                new Vector2Int(x, y - 1), // 0
                new Vector2Int(isEvenCol ? x - 1 : x + 1, isEvenCol ? y - x % 2 : y + 1), // 1
                new Vector2Int(x + 1, y - x % 2), // 2
                new Vector2Int(isEvenCol ? x + 1 : x - 1, isEvenCol ? y - 1 : y + 1 - x % 2), // 3
                new Vector2Int(x + 1, y + 1 - x % 2), // 4
                new Vector2Int(x, y + 1) // 5
            };
        }


        public static bool[,] GetTerritory(IReadOnlyDictionary<Vector2Int, MapObject> map, Vector2Int size, MapObjectType type)
        {
            bool[,] territory = MathUtilities.EmptyMatrix(size, false);
            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    if (!map.ContainsKey(new Vector2Int(x, y))) continue;
                    if (map[new Vector2Int(x, y)].Type != type) continue;

                    territory[x, y] = true;
                }
            }

            return territory;
        }


        public static List<Island> GetIslands(bool[,] map, Vector2Int size)
        {
            List<Island> islands = new List<Island>();

            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    if (!map[x, y]) continue;
                    if (islands.Exists(i => i.Tiles.Contains(new Vector2Int(x, y)))) continue;

                    Island island = GetIsland(map, size, new Vector2Int(x, y));
                    islands.Add(island);
                }
            }

            return islands;
        }


        private static Island GetIsland(bool[,] map, Vector2Int size, Vector2Int position)
        {
            bool[,] island = MathUtilities.EmptyMatrix(size, false);
            GetIslandStep(map, size, position, island);

            return new Island(island.ToList());
        }


        private static void GetIslandStep(bool[,] fromMap, Vector2Int size, Vector2Int position, bool[,] island)
        {
            if (OutOfBounds(position, size) || !fromMap[position.x, position.y] || island[position.x, position.y])
            {
                return;
            }


            island[position.x, position.y] = true;
            Vector2Int[] neighbours = GetNeighbours(position);
            foreach (Vector2Int neighbour in neighbours)
            {
                if (!OutOfBounds(neighbour, size) && fromMap[neighbour.x, neighbour.y])
                {
                    GetIslandStep(fromMap, size, neighbour, island);
                }
            }
        }


        public static List<Vector2Int> GetIslandShore(IReadOnlyDictionary<Vector2Int, MapObject> map, Island island, Vector2Int size)
        {
            List<Vector2Int> shore = new List<Vector2Int>();

            for (int i = 0; i < island.Tiles.Count; i++)
            {
                Vector2Int tile = island.Tiles[i];
                Vector2Int[] neighbours = GetNeighbours(tile);
                foreach (Vector2Int neighbour in neighbours)
                {
                    if (OutOfBounds(neighbour, size) || !map.ContainsKey(neighbour) || island.Tiles.Contains(neighbour))
                    {
                        continue;
                    }

                    shore.Add(tile);
                    break;
                }
            }

            return shore;
        }

    }
}
