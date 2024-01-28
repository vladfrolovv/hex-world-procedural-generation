#region

#endregion

using System.Collections.Generic;
using UnityEngine;
namespace Game.Runtime.UtilitiesContainer
{
    public static class Utilities
    {

        public static int Step(this float value, float threshold)
        {
            return value < threshold ? 0 : 1;
        }


        public static IEnumerable<Vector2Int> ToList(this bool[,] map)
        {
            (int width, int height) = GetMatrixDimensions(map);
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


        private static (int height, int width) GetMatrixDimensions<T>(T[,] matrix)
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
