#region

using System.Collections.Generic;
using System.Linq;
using Core.Runtime.Base;
using Game.Runtime.Cameras;
using Game.Runtime.Islands;
using Game.Runtime.Logger;
using Game.Runtime.Maps.MapObjects;
using Game.Runtime.UtilitiesContainer;
using UnityEngine;
using Zenject;

#endregion

namespace Game.Runtime.Maps
{
    public class MapGenerator : BaseBehaviour
    {

        [SerializeField] private Vector2Int _mapSize;
        [Range(0f, 1f)]
        [SerializeField] private float _perlinInterpolator;
        [Range(.01f, 25f)]
        [SerializeField] private float _perlinScale;
        [SerializeField] private bool _updateOnValidate;

        private readonly Dictionary<Vector2Int, MapObject> _mapObjects =
            new Dictionary<Vector2Int, MapObject>();
        private CameraInstaller _cameraInstaller;
        private DiContainer _diContainer;

        private MapObjectsConfig _mapObjectsConfig;


        protected void OnEnable()
        {
            Generate();
        }


        protected void OnValidate()
        {
            if (Application.isPlaying && _updateOnValidate)
            {
                Generate();
            }
        }


        public void Generate()
        {
            ClearMap();
            CreateMap();
            GenerateWaterBorder();
            DefineIslands();

            _cameraInstaller.Install(_mapSize);
        }


        private void CreateMap()
        {
            float[,] combinedMap = PerlinUtilities.GeneratePerlinRadialGradientMap(_mapSize, _perlinScale);
            float threshold = combinedMap.GetThreshold(_perlinInterpolator);

            for (int y = 0; y < _mapSize.y; y++)
            {
                for (int x = 0; x < _mapSize.x; x++)
                {
                    if (combinedMap[x, y] < threshold) continue;
                    AddObject(new Vector2Int(x, y), MapObjectType.Grass);
                }
            }

        }


        private void GenerateWaterBorder()
        {
            for (int y = 0; y < _mapSize.y; y++)
            {
                for (int x = 0; x < _mapSize.x; x++)
                {
                    Vector2Int position = new Vector2Int(x, y);
                    if (!_mapObjects.ContainsKey(position)) continue;
                    if (_mapObjects[position].Type != MapObjectType.Grass) continue;

                    foreach (Vector2Int neighbor in MapUtilities.GetNeighbours(position))
                    {
                        if (_mapObjects.ContainsKey(neighbor)) continue;

                        AddObject(neighbor, MapObjectType.Water);
                    }
                }
            }
        }


        private void DefineIslands()
        {
            bool[,] territory = MapUtilities.GetTerritory(_mapObjects, _mapSize, MapObjectType.Grass);
            Island island = MapUtilities.GetIsland(territory, _mapSize, new Vector2Int(Mathf.FloorToInt(_mapSize.x / 2), Mathf.FloorToInt(_mapSize.y / 2)));
        }


        private void ClearMap()
        {
            _mapObjects.Values.ToList().ForEach(mapObject => Destroy(mapObject.gameObject));
            _mapObjects.Clear();
        }


        private void AddObject(Vector2Int position, MapObjectType objectType, float yOffset = 0f)
        {
            if (_mapObjects.Keys.Contains(position)) return;

            MapObject prefab = _mapObjectsConfig.GetObjectPrefab(objectType);
            MapObject mapObject = _diContainer.InstantiatePrefabForComponent<MapObject>(prefab, transform);
            mapObject.Type = objectType;

            mapObject.transform.position = position.ToWorldPosition(yOffset);
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
