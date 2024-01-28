#region

using System.Numerics;
using Core.Runtime.Base;
using Game.Runtime.Maps.MapObjects;
using UnityEngine;

#endregion

namespace Game.Runtime.Maps
{
    public class MapObject : BaseBehaviour
    {
        public MapObjectType Type { get; set; }

        [field: SerializeField]
        public Vector2Int StandardDirection { get; private set; }
    }
}
