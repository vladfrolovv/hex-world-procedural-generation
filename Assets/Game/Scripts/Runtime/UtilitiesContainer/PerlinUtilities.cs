#region

using Game.Runtime.Logger;
using UnityEngine;

#endregion

namespace Game.Runtime.UtilitiesContainer
{
    public static class PerlinUtilities
    {

        public static float[,] GeneratePerlinNoiseMap(Vector2Int mapSize, float scale)
        {
            float[,] map = new float[mapSize.x, mapSize.y];
            Vector2 perlinOffset = new Vector2(
                Random.Range(0f, Mathf.Pow(2, 8)),
                Random.Range(0f, Mathf.Pow(2, 8)));

            for (int y = 0; y < mapSize.y; y++)
            {
                for (int x = 0; x < mapSize.x; x++)
                {
                    float perlinX = ((float)x / mapSize.x + perlinOffset.x) * scale;
                    float perlinY = ((float)y / mapSize.y + perlinOffset.y) * scale;

                    map[x, y] = Mathf.PerlinNoise(perlinX, perlinY);
                }
            }

            LoggingUtilities.LogMatrix("Perlin map: ", map, LogColor.green);
            return map;
        }


        public static float[,] GeneratePerlinRadialGradientMap(Vector2Int mapSize, float perlinScale)
        {
            float[,] perlinMap = GeneratePerlinNoiseMap(mapSize, perlinScale);
            float[,] gradientMap = RadialGradientMap(mapSize).Normalize();
            float[,] map = new float[mapSize.x, mapSize.y];

            for (int y = 0; y < mapSize.y; y++)
            {
                for (int x = 0; x < mapSize.x; x++)
                {
                    map[x, y] = perlinMap[x, y] * Mathf.Clamp01(1f - gradientMap[x, y]);
                }
            }

            LoggingUtilities.LogMatrix("Combined map: ", map, LogColor.green);
            return map;
        }


        public static float[,] RadialGradientMap(Vector2Int mapSize)
        {
            float[,] gradientMap = new float[mapSize.x, mapSize.y];
            Vector2Int center = new Vector2Int(mapSize.x / 2, mapSize.y / 2);

            for (int y = 0; y < mapSize.y; y++)
            {
                for (int x = 0; x < mapSize.x; x++)
                {
                    float distance = Vector2Int.Distance(center, new Vector2Int(x, y));
                    gradientMap[x, y] = distance;
                }
            }

            LoggingUtilities.LogMatrix("Gradient map: ", gradientMap, LogColor.green);
            return gradientMap;
        }
    }
}
