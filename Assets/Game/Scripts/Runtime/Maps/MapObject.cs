#region

using Core.Runtime.Base;
using Game.Runtime.Maps.MapObjects;

#endregion

namespace Game.Runtime.Maps
{
    public class MapObject : BaseBehaviour
    {
        public MapObjectType Type { get; set; }
    }
}
