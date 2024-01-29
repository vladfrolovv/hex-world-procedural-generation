#region

using System.Collections.Generic;
using System.Linq;
using Core.Runtime.Base;
using Game.Runtime.Cameras;
using Game.Runtime.Islands;
using Game.Runtime.Logger;
using Game.Runtime.Maps.MapObjects;
using Game.Runtime.UtilitiesContainer;
using UnityEditor;
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

        private List<Island> _islands = new List<Island>();

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
            GenerateRiver();

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
                    Vector2Int pos = new Vector2Int(x, y);
                    if (combinedMap[pos.x, pos.y] < threshold) continue;
                    AddObject(new Vector2Int(pos.x, pos.y), MapObjectType.Grass);
                }
            }

            _islands = new List<Island>(
                MapUtilities.GetIslands(MapUtilities.GetTerritory(_mapObjects, _mapSize, MapObjectType.Grass), _mapSize)
                    .OrderBy(island => island.Tiles.Count));
        }


        private void GenerateWaterBorder()
        {
            for (int y = 0; y < _mapSize.y; y++)
            {
                for (int x = 0; x < _mapSize.x; x++)
                {
                    Vector2Int position = new Vector2Int(x, y);
                    _mapObjects.TryGetValue(position, out MapObject mapObject);
                    if (mapObject == null || mapObject.Type != MapObjectType.Grass) continue;

                    foreach (Vector2Int neighbor in MapUtilities.GetNeighbours(position))
                    {
                        _mapObjects.TryGetValue(neighbor, out MapObject neighborObject);
                        if (neighborObject != null && neighborObject.Type == MapObjectType.Grass) continue;

                        AddObject(neighbor, MapObjectType.Water);
                    }
                }
            }
        }


        private void GenerateRiver()
        {
            Island onIsland = _islands.LastOrDefault();
            if (onIsland == null) return;

            List<Vector2Int> shore = MapUtilities.GetIslandShore(_mapObjects, onIsland, _mapSize);
            List<Vertex> vertices = NavigationUtilities.GetVertices(_mapObjects, shore);
            LoggingUtilities.Log($"Trying to generate river from shore [{shore.Count}] With vertices [{vertices.Count}]...");
        }


        private void ClearMap()
        {
            _mapObjects.Values.ToList().ForEach(mapObject => Destroy(mapObject.gameObject));
            _mapObjects.Clear();
            _islands.Clear();
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
