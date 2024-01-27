#region

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Game.Runtime.Islands
{
    public class Island
    {

        public Island(IEnumerable<Vector2Int> tiles)
        {
            Tiles = new List<Vector2Int>(tiles);
        }


        public List<Vector2Int> Tiles { get; }
    }
}
