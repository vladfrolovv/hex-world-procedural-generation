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

        private const float HexRadius = .577f;
        private const float CenterMultiplier = .865f;

        public static Vector3 ToWorldPosition(this Vector2Int position, float z = 0f)
        {
            float offsetX = position.x * Mathf.Sqrt(3) * HexRadius;
            float offsetY = position.y * 1.5f * HexRadius;

            if (position.y % 2 != 0)
            {
                offsetX += HexRadius * CenterMultiplier;
            }

            return new Vector3(offsetX, z, offsetY);
        }

        public static bool OutOfBounds(Vector2Int position, Vector2Int mapSize)
        {
            return !(position.x >= 0 && position.x < mapSize.x && position.y >= 0 && position.y < mapSize.y);
        }


        public static Vector2Int[] GetNeighbours(Vector2Int position)
        {
            return GetNeighbours(position.x, position.y);
        }


        public static Vector2Int[] GetNeighbours(int x, int y)
        {
            return new Vector2Int[6]
            {
                new Vector2Int(x, y - 1),
                new Vector2Int(x - 1, y - x % 2),
                new Vector2Int(x + 1, y - x % 2),
                new Vector2Int(x - 1, y + 1 - x % 2),
                new Vector2Int(x + 1, y + 1 - x % 2),
                new Vector2Int(x, y + 1)
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


        public static Island GetIsland(bool[,] map, Vector2Int size, Vector2Int position)
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


        private static IEnumerable<Vector2Int> ToList(this bool[,] map)
        {
            (int height, int width) = GetMatrixDimensions(map);
            List<Vector2Int> list = new List<Vector2Int>();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (map[x, y])
                    {
                        list.Add(new Vector2Int(x, y));
                    }
                }
            }

            return list;
        }


        public static (int height, int width) GetMatrixDimensions<T>(T[,] matrix)
        {
            if (matrix == null)
            {
                return (0, 0);
            }
            int height = matrix.GetLength(0);
            if (height <= 0)
            {
                return (0, 0);
            }
            int width = matrix.GetLength(1);
            return (height, width);
        }

    }
}
