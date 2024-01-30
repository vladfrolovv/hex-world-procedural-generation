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

        [SerializeField] private Vector2Int _mapSize = new Vector2Int(25, 25);
        [Range(0f, 1f)]
        [SerializeField] private float _perlinInterpolator = .5f;
        [Range(.01f, 25f)]
        [SerializeField] private float _perlinScale = 5f;
        [Range(.01f, 1f)]
        [SerializeField] private float _riverInterpolator = .9f;
        [SerializeField] private bool _updateOnValidate;

        private readonly Dictionary<Vector2Int, MapObject> _mapObjects =
            new Dictionary<Vector2Int, MapObject>();

        private List<Island> _islands = new List<Island>();

        private float[,] _perlinMap;

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
            _perlinMap = PerlinUtilities.GeneratePerlinRadialGradientMap(_mapSize, _perlinScale);
            float threshold = _perlinMap.GetThreshold(_perlinInterpolator);

            for (int y = 0; y < _mapSize.y; y++)
            {
                for (int x = 0; x < _mapSize.x; x++)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    if (_perlinMap[pos.x, pos.y] < threshold) continue;
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

            if (onIsland == null)
            {
                LoggingUtilities.Log("No islands found.", LogColor.red);
                return;
            }

            List<Vector2Int> islandTiles = new List<Vector2Int>(onIsland.Tiles);
            List<Vector2Int> shoreTiles = MapUtilities.GetIslandShore(_mapObjects, onIsland, _mapSize);

            islandTiles.RemoveAll(tile => shoreTiles.Contains(tile));
            Vector2Int? fromNullable = null;
            while (fromNullable == null)
            {
                fromNullable = islandTiles[Random.Range(0, islandTiles.Count)];
                List<Vector2Int> neighbours = MapUtilities.GetNeighbours((Vector2Int)fromNullable).ToList();
                bool shoreNearby = neighbours.Any(n => shoreTiles.Contains(n));
                if (!shoreNearby)
                    fromNullable = null;
            }

            Vector2Int from = (Vector2Int)fromNullable;
            Vector2Int to = MapUtilities.DistanceMap(islandTiles, from, _mapSize).LargestDistanceMapValue(_mapSize);
            List<Vector2Int> path = NavigationUtilities.GetPath(NavigationUtilities.GetVertices(_mapObjects, onIsland.Tiles), from, to);
            if (path.Count == 0)
            {
                LoggingUtilities.Log("No path found.", LogColor.red);
                return;
            }

            LoggingUtilities.Log($"Trying to create river from {from}, {to}", LogColor.green);
            path.ForEach(tile => _mapObjects[tile].transform.position = tile.ToWorldPosition(.1f));
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
