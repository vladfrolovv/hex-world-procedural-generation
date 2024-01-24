#region

using System;
using Core.Runtime.Base;
using Core.Runtime.Containers;
using UnityEngine;

#endregion

namespace Game.Runtime.Maps.MapObjects
{
    [CreateAssetMenu(fileName = "Map Objects Config", menuName = "Game/Runtime/Map/Map Objects")]
    public class MapObjectsConfig : BaseScriptableObject
    {

        [SerializeField] private MapObject _defaultElementPrefab;
        [SerializeField] private ObjectsMap _objectsMap;

        public MapObject GetObjectPrefab(MapObjectType objectType)
        {
            return _objectsMap.Keys.Contains(objectType) ? _objectsMap[objectType] : _defaultElementPrefab;
        }


        [Serializable]
        private class ObjectsMap : KeyValueList<MapObjectType, MapObject>
        {
        }

    }
}
