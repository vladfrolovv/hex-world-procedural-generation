#region

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Game.Runtime.UtilitiesContainer
{
    public class Vertex
    {
        public Vertex(Vector2Int position, IEnumerable<Vector2Int> connectedWith)
        {
            Position = position;
            ConnectedWith = new List<Vector2Int>(connectedWith);
        }

        public Vector2Int Position { get; }
        public List<Vector2Int> ConnectedWith { get; }
    }
}
