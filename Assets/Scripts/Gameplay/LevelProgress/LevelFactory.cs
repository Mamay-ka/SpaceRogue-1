using System;
using Gameplay.Enemy;
using Gameplay.Player;
using Gameplay.Services;
using Gameplay.Space.Factories;
using Gameplay.Space.Generator;
using Gameplay.Space.Obstacle;
using Gameplay.Space.SpaceObjects.Scriptables;
using Scriptables;
using Zenject;

namespace Gameplay.LevelProgress
{
    public sealed class LevelFactory : PlaceholderFactory<int, Level>
    {
        private readonly LevelPresetsConfig _levelPresetsConfig;
        private readonly SpaceViewFactory _spaceViewFactory;
        private readonly MapGeneratorFactory _mapGeneratorFactory;
        private readonly LevelMapFactory _levelMapFactory;
        private readonly SpawnPointsFinderFactory _spawnPointsFinderFactory;
        private readonly SpaceObstacleFactory _spaceObstacleFactory;
        private readonly PlayerFactory _playerFactory;
        private readonly StarSpawnConfig _starSpawnConfig;
        private readonly PlanetSpawnConfig _planetSpawnConfig;
        private readonly SpaceObjectFactory _spaceObjectFactory;
        private readonly EnemyForcesFactory _enemyForcesFactory;
        
        private LevelPreset _currentLevelPreset;

        public event Action<Level> LevelCreated = (_) => { };

        public LevelFactory(
            LevelPresetsConfig levelPresetsConfig,
            SpaceViewFactory spaceViewFactory,
            MapGeneratorFactory mapGeneratorFactory,
            LevelMapFactory levelMapFactory,
            SpawnPointsFinderFactory spawnPointsFinderFactory,
            SpaceObstacleFactory spaceObstacleFactory,
            PlayerFactory playerFactory,
            StarSpawnConfig starSpawnConfig,
            PlanetSpawnConfig planetSpawnConfig,
            SpaceObjectFactory spaceObjectFactory,
            EnemyForcesFactory enemyForcesFactory)
        {
            _levelPresetsConfig = levelPresetsConfig;
            _spaceViewFactory = spaceViewFactory;
            _mapGeneratorFactory = mapGeneratorFactory;
            _levelMapFactory = levelMapFactory;
            _spawnPointsFinderFactory = spawnPointsFinderFactory;
            _spaceObstacleFactory = spaceObstacleFactory;
            _playerFactory = playerFactory;
            _starSpawnConfig = starSpawnConfig;
            _planetSpawnConfig = planetSpawnConfig;
            _spaceObjectFactory = spaceObjectFactory;
            _enemyForcesFactory = enemyForcesFactory;
        }

        public override Level Create(int levelNumber)
        {
            _currentLevelPreset = PickRandomLevelPreset();
            var spaceView = _spaceViewFactory.Create();

            var map = _mapGeneratorFactory.Create(_currentLevelPreset.SpaceConfig);
            map.Generate();

            var levelMap = _levelMapFactory.Create(spaceView, _currentLevelPreset.SpaceConfig, map.BorderMap, map.NebulaMap);
            levelMap.Draw();
            var mapCameraSize = levelMap.GetMapCameraSize();

            var spawnPointsFinder = _spawnPointsFinderFactory.Create(map.NebulaMap, spaceView.NebulaTilemap);

            _spaceObstacleFactory.Create(spaceView.SpaceObstacleView, _currentLevelPreset.SpaceConfig.ObstacleForce);

            var player = _playerFactory.Create(spawnPointsFinder.GetPlayerSpawnPoint());

            //_spaceObjectFactory

            var enemyForces = _enemyForcesFactory.Create(_currentLevelPreset.SpaceConfig.EnemyGroupCount, spawnPointsFinder);

            var level = new Level(levelNumber, _currentLevelPreset.EnemiesCountToWin, spaceView, mapCameraSize, player, enemyForces);
            LevelCreated.Invoke(level);
            return level;
        }

        private LevelPreset PickRandomLevelPreset()
        {
            var index = new Random().Next(_levelPresetsConfig.Presets.Count);
            return _levelPresetsConfig.Presets[index];
        }
    }
}