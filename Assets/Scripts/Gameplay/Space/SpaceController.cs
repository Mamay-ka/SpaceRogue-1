using System.Collections.Generic;
using System.Linq;
using Abstracts;
using Gameplay.Space.Generator;
using Gameplay.Space.Obstacle;
using Gameplay.Space.Planet;
using Scriptables.Enemy;
using Scriptables.Space;
using UnityEngine;
using Utilities.ResourceManagement;

namespace Gameplay.Space
{
    public sealed class SpaceController : BaseController
    {
        private readonly ResourcePath _viewPath = new(Constants.Prefabs.Gameplay.Space.Level);
        private readonly ResourcePath _configPath = new(Constants.Configs.Space.SpaceConfig);

        private readonly ResourcePath _starSpawnConfigPath = new(Constants.Configs.Space.DefaultStarSpawn);
        private readonly ResourcePath _planetSpawnConfigPath = new(Constants.Configs.Space.DefaultPlanetSpawn);
        private readonly ResourcePath _groupSpawnConfigPath = new(Constants.Configs.Enemy.EnemySpawnConfig);

        private readonly SpaceView _view;
        private readonly SpaceConfig _config;
        private readonly SpaceObjectFactory _spaceObjectFactory;
        private readonly LevelGenerator _levelGenerator;

        public SpaceController()
        {
            _view = LoadView<SpaceView>(_viewPath);
            _config = ResourceLoader.LoadObject<SpaceConfig>(_configPath);
            var starSpawnConfig = ResourceLoader.LoadObject<StarSpawnConfig>(_starSpawnConfigPath);
            var planetSpawnConfig = ResourceLoader.LoadObject<PlanetSpawnConfig>(_planetSpawnConfigPath);
            var enemySpawnConfig = ResourceLoader.LoadObject<EnemySpawnConfig>(_groupSpawnConfigPath);

            _spaceObjectFactory = new SpaceObjectFactory(starSpawnConfig, planetSpawnConfig);

            _levelGenerator = new(_view, _config, starSpawnConfig, enemySpawnConfig);
            _levelGenerator.Generate();
            AddObstacleController(_view.ObstacleView, _config.ObstacleForce);

            foreach (var starSpawnPoint in _levelGenerator.GetSpawnPoints(CellType.Star))
            {
                var (star, planetControllers) = _spaceObjectFactory.CreateStarSystem(starSpawnPoint, _view.Stars);
                AddController(star);
                AddPlanetControllers(planetControllers);
            }
        }

        public Vector3 GetPlayerSpawnPoint()
        {
            return _levelGenerator.GetPlayerSpawnPoint();
        }

        public List<Vector3> GetEnemySpawnPoints()
        {
            return _levelGenerator.GetSpawnPoints(CellType.Enemy);
        }

        public float GetMapCameraSize()
        {
            var maxCellCount = Mathf.Max(_config.WidthMap, _config.HeightMap);
            var maxCellSize = Mathf.Max(_view.NebulaTilemap.cellSize.x, _view.NebulaTilemap.cellSize.y);
            return maxCellCount * maxCellSize / 2;
        }

        private void AddPlanetControllers(PlanetController[] planetControllers)
        {
            if (!planetControllers.Any()) return;
            foreach (var planet in planetControllers)
            {
                AddController(planet);
            }
        }

        private void AddObstacleController(ObstacleView obstacleView, float obstacleForce)
        {
            var obstacleController = new ObstacleController(obstacleView, obstacleForce);
            AddController(obstacleController);
        }
    }
}