#region

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
            return position.x >= 0 && position.x < mapSize.x && position.y >= 0 && position.y < mapSize.y;
        }


        public static Vector2Int[] GetNeighbours(Vector2Int position)
        {
            return GetNeighbours(position.x, position.y);
        }


        public static Vector2Int[] GetNeighbours(int x, int y)
        {
            return new Vector2Int[6] {
                new Vector2Int(x, y - 1),
                new Vector2Int(x - 1, y - x % 2),
                new Vector2Int(x + 1, y - x % 2),
                new Vector2Int(x - 1, y + 1 - x % 2),
                new Vector2Int(x + 1, y + 1 - x % 2),
                new Vector2Int(x, y + 1)
            };
        }

    }
}
