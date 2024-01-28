#region

using System.Collections.Generic;
using System.Linq;
using Game.Runtime.Islands;
using Game.Runtime.Maps;
using Game.Runtime.Maps.MapObjects;
using UnityEngine;

#endregion

namespace Game.Runtime.UtilitiesContainer
{
    public static class MathUtilities
    {

        public static float[,] Normalize(this float[,] map)
        {
            float min = map.Cast<float>().Min();
            float max = map.Cast<float>().Max();
            for (int y = 0; y < map.GetLength(1); y++)
            {
                for (int x = 0; x < map.GetLength(0); x++)
                {
                    map[x, y] = (map[x, y] - min) / (max - min);
                }
            }

            return map;
        }


        public static T[,] EmptyMatrix<T>(Vector2Int size, T defaultValue)
        {
            T[,] matrix = new T[size.x, size.y];
            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    matrix[x, y] = defaultValue;
                }
            }

            return matrix;
        }


        public static float GetThreshold(this float[,] map, float interpolator)
        {
            return Mathf.Lerp(map.Cast<float>().Min(), map.Cast<float>().Max(), interpolator);
        }


        public static (T, T) SelectPair<T>(List<T> list)
        {
            int index1 = Random.Range(0, list.Count);
            int index2 = Random.Range(0, list.Count - 1);
            if (index2 >= index1)
            {
                index2++;
            }

            return (list[index1], list[index2]);
        }
    }
}
