using Gameplay.Space.Generator;
using Gameplay.Space.SpaceObjects.Scriptables;
using System.Linq;
using UnityEngine;
using Utilities.Mathematics;

namespace DebugLevelGenerator.Editor
{
    public sealed class DebugLevelGenerator
    {
        private const float DefaultOrbit = 10;

        private readonly DebugLevelGeneratorView _debugLevelGeneratorView;
        private readonly float _maxRealCellSize;

        public DebugLevelGenerator(DebugLevelGeneratorView debugLevelGeneratorView)
        {
            _debugLevelGeneratorView = debugLevelGeneratorView;

            var tilemap = _debugLevelGeneratorView.SpaceView.NebulaTilemap;
            _maxRealCellSize = Mathf.Max(
                tilemap.cellSize.x * tilemap.transform.localScale.x,
                tilemap.cellSize.y * tilemap.transform.localScale.y);
        }

        public void Generate()
        {
            ClearTileMaps();

            var map = new MapGenerator(_debugLevelGeneratorView.SpaceConfig);
            map.Generate();
            
            var debugLevelMap = new DebugLevelMap(_debugLevelGeneratorView, _debugLevelGeneratorView.SpaceConfig, map.BorderMap, map.NebulaMap);

            var spawnPointsFinder = new SpawnPointsFinder(map.NebulaMap, _debugLevelGeneratorView.SpaceView.NebulaTilemap);

            var random = new System.Random();

            for (int i = 0; i < _debugLevelGeneratorView.SpaceConfig.SpaceObjectCount; i++)
            {
                var config = RandomPicker.PickOneElementByWeights(_debugLevelGeneratorView.SpaceObjectSpawnConfig.SpaceObjectWeights, random);
                var starSize = RandomPicker.PickRandomBetweenTwoValues(config.MinSize, config.MaxSize, random);
                var effectConfig = config.Effects.First(x => x is PlanetSystemConfig planetSystemConfig);
                
                var orbit = DefaultOrbit;
                if (effectConfig is PlanetSystemConfig planetSystemConfig)
                {
                    orbit = planetSystemConfig.MinOrbit;
                }
                
                if (spawnPointsFinder.TryGetSpaceObjectSpawnPoint(starSize, orbit, out var spawnPoint))
                {
                    debugLevelMap.SetSpaceObjectsTile(CorrectCoordinate(spawnPoint));
                }
            }
        }

        public void ClearTileMaps()
        {
            if (_debugLevelGeneratorView.SpaceView.BorderTilemap != null)
            {
                _debugLevelGeneratorView.SpaceView.BorderTilemap.ClearAllTiles();
                _debugLevelGeneratorView.SpaceView.BorderMaskTilemap.ClearAllTiles();
            }
            if (_debugLevelGeneratorView.SpaceView.NebulaTilemap != null)
            {
                _debugLevelGeneratorView.SpaceView.NebulaTilemap.ClearAllTiles();
                _debugLevelGeneratorView.SpaceView.NebulaMaskTilemap.ClearAllTiles();
            }
            if (_debugLevelGeneratorView.SpaceObjectsTilemap != null)
            {
                _debugLevelGeneratorView.SpaceObjectsTilemap.ClearAllTiles();
            }
        }

        private Vector3Int CorrectCoordinate(Vector3 point)
        {
            return Vector3Int.FloorToInt(point / _maxRealCellSize);
        }
    }
}