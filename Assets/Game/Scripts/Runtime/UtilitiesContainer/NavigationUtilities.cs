#region

using System.Collections.Generic;
using System.Linq;
using Game.Runtime.Maps;
using UnityEngine;

#endregion

namespace Game.Runtime.UtilitiesContainer
{
    public static class NavigationUtilities
    {

        public static List<Vertex> GetVertices(IReadOnlyDictionary<Vector2Int, MapObject> map, List<Vector2Int> territory)
        {
            List<Vertex> vertices = new List<Vertex>();
            foreach (var position in territory)
            {
                vertices.Add(new Vertex(position, MapUtilities.GetNeighbours(position).Where(p => territory.Contains(p))));
            }

            return vertices;
        }

        public static List<Vector2Int> GetPath(List<Vertex> vertices, Vector2Int start, Vector2Int goal)
        {
            if (vertices.Count == 0) return new List<Vector2Int>();

            Dictionary<Vector2Int, Vector2Int> previous = new Dictionary<Vector2Int, Vector2Int>();
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                Vector2Int current = queue.Dequeue();

                if (current == goal)
                {
                    Queue<Vector2Int> path = new Queue<Vector2Int>();
                    while (previous.ContainsKey(current))
                    {
                        path.Enqueue(current);
                        current = previous[current];
                    }
                    path.Enqueue(start);
                    path = new Queue<Vector2Int>(path.Reverse());
                    return path.ToList();
                }

                foreach (var neighbor in vertices.Find(v => v.Position == current).ConnectedWith)
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                        previous[neighbor] = current;
                    }
                }
            }

            return new List<Vector2Int>();
        }
    }
}
