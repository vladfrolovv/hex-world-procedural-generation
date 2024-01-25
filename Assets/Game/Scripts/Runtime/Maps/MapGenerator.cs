#region

using System.Collections.Generic;
using System.Linq;
using Core.Runtime.Base;
using Game.Runtime.Cameras;
using Game.Runtime.Maps.MapObjects;
using Game.Runtime.UtilitiesContainer;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

#endregion

namespace Game.Runtime.Maps
{
    public class MapGenerator : BaseBehaviour
    {
        private readonly Dictionary<Vector2Int, MapObject> _mapObjects =
            new Dictionary<Vector2Int, MapObject>();

        private MapObjectsConfig _mapObjectsConfig;
        private DiContainer _diContainer;
        private CameraInstaller _cameraInstaller;


        [SerializeField] private Vector2Int _mapSize;


        protected void OnEnable()
        {
            Generate();
        }


        public void Generate()
        {
            ClearMap();
            CreateMap();

            _cameraInstaller.Install(_mapSize);
        }


        private void CreateMap()
        {
            for (int y = 0; y < _mapSize.y; y++)
            {
                for (int x = 0; x < _mapSize.x; x++)
                {
                    Vector2Int position = new Vector2Int(x, y);
                    AddObject(position, MapObjectType.Water);
                }
            }


            for (int i = 0; i < _mapSize.x; i++)
            {
                ReplaceObject(new Vector2Int(Random.Range(0, _mapSize.x), Random.Range(0, _mapSize.y)), MapObjectType.Grass);
            }

            Utilities.GetDistanceMap(_mapSize);
        }


        private void ClearMap()
        {
            _mapObjects.Values.ToList().ForEach(mapObject => Destroy(mapObject.gameObject));
            _mapObjects.Clear();
        }


        private void AddObject(Vector2Int position, MapObjectType objectType)
        {
            if (_mapObjects.Keys.Contains(position)) return;

            MapObject prefab = _mapObjectsConfig.GetObjectPrefab(objectType);
            MapObject mapObject = _diContainer.InstantiatePrefabForComponent<MapObject>(prefab, transform);

            mapObject.transform.position = position.ToWorldPosition();
            _mapObjects.Add(position, mapObject);
        }


        private void ReplaceObject(Vector2Int position, MapObjectType newObjectType)
        {
            RemoveObject(position);
            AddObject(position, newObjectType);
        }


        private void RemoveObject(Vector2Int from)
        {
            if (!_mapObjects.Keys.Contains(from)) return;

            DestroyImmediate(_mapObjects[from].gameObject);
            _mapObjects.Remove(from);
        }


        [Inject]
        public void Construct(MapObjectsConfig mapObjectsConfig, DiContainer diContainer,
                              CameraInstaller cameraInstaller)
        {
            _mapObjectsConfig = mapObjectsConfig;
            _diContainer = diContainer;
            _cameraInstaller = cameraInstaller;
        }

    }
}
