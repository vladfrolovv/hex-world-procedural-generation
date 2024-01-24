#region

using System;
using System.Linq;
using Game.Runtime.Logger;
using UnityEngine;

#endregion

namespace Game.Runtime.UtilitiesContainer
{
    public static class Utilities
    {

        public static float[,] GetDistanceMap(Vector2Int size, Vector2Int? to = null)
        {
            float[,] distanceMap = new float[size.x, size.y];
            Vector2Int toPosition = to ?? new Vector2Int(size.x / 2, size.y / 2);
            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    Vector2Int position = new Vector2Int(x, y);
                    distanceMap[x, y] = Vector2.Distance(position, toPosition);
                }
            }

            float maxValue = distanceMap.Cast<float>().Max();
            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    distanceMap[x, y] /= maxValue;
                    distanceMap[x, y] = (float)Math.Round(1 - distanceMap[x, y], 2);
                }
            }

            LoggingUtilities.LogMatrix($"Distance map: ", distanceMap, LogColor.green);
            return distanceMap;
        }


        private static float Step(this float value, float threshold)
        {
            return value < threshold ? 0f : 1f;
        }

    }
}
