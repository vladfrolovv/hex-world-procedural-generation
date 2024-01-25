#region

using System.Linq;
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


        public static float GetThreshold(this float[,] map, float interpolator)
        {
            return Mathf.Lerp(map.Cast<float>().Min(), map.Cast<float>().Max(), interpolator);
        }
    }
}
