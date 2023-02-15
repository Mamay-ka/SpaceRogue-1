using Abstracts;
using Gameplay.Player;
using Scriptables.Enemy;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Mathematics;
using Utilities.ResourceManagement;
using Utilities.Unity;

namespace Gameplay.Enemy
{
    public sealed class EnemyForcesController : BaseController
    {
        private const byte MaxCountSpawnTries = 10;
        
        private readonly ResourcePath _groupSpawnConfigPath = new(Constants.Configs.Enemy.EnemySpawnConfig);
        private readonly EnemyFactory _enemyFactory;
        private readonly PlayerController _playerController;
        
        public List<EnemyView> EnemyViews { get; private set; } = new();

        public EnemyForcesController(PlayerController playerController, List<Vector3> enemySpawnPoints)
        {
            _playerController = playerController;
            var groupSpawnConfig = ResourceLoader.LoadObject<EnemySpawnConfig>(_groupSpawnConfigPath);

            _enemyFactory = new EnemyFactory(groupSpawnConfig.Enemy);

            var unitSize = groupSpawnConfig.Enemy.Prefab.transform.localScale;

            var countPoints = new List<EnemyGroupSpawn>(groupSpawnConfig.EnemyGroupsSpawnPoints);
            foreach (var spawnPoint in enemySpawnPoints)
            {
                var count = new System.Random().Next(countPoints.Count);
                var spawnCircleRadius = groupSpawnConfig.EnemyGroupsSpawnPoints[count].GroupCount * 2;
                for (int i = 0; i < groupSpawnConfig.EnemyGroupsSpawnPoints[count].GroupCount; i++)
                {
                    var unitSpawnPoint = GetEmptySpawnPoint(spawnPoint, unitSize, spawnCircleRadius);
                    var enemyController = _enemyFactory.CreateEnemy(unitSpawnPoint, _playerController);
                    EnemyViews.Add(enemyController.View);
                    AddController(enemyController);
                }
                countPoints.Remove(groupSpawnConfig.EnemyGroupsSpawnPoints[count]);
            }
            countPoints.Clear();
        }

        protected override void OnDispose()
        {
            EnemyViews.Clear();
        }

        private static Vector3 GetEmptySpawnPoint(Vector3 spawnPoint, Vector3 unitSize, int spawnCircleRadius)
        {
            var unitSpawnPoint = spawnPoint + (Vector3)(Random.insideUnitCircle * spawnCircleRadius);
            var unitMaxSize = unitSize.MaxVector3CoordinateOnPlane();
            
            var tryCount = 0;
            while (UnityHelper.IsAnyObjectAtPosition(unitSpawnPoint, unitMaxSize) && tryCount <= MaxCountSpawnTries)
            {
                unitSpawnPoint = spawnPoint + (Vector3)(Random.insideUnitCircle * spawnCircleRadius);
                tryCount++;
            }

            return unitSpawnPoint;
        }
    }
}