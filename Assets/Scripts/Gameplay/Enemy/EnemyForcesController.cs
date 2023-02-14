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
        private readonly ResourcePath _groupSpawnConfigPath = new(Constants.Configs.Enemy.EnemySpawnConfig);
        private readonly EnemyFactory _enemyFactory;
        private readonly PlayerController _playerController;

        public EnemyForcesController(PlayerController playerController, List<Vector3> enemySpawnPoints)
        {
            _playerController = playerController;
            var groupSpawnConfig = ResourceLoader.LoadObject<EnemySpawnConfig>(_groupSpawnConfigPath);

            _enemyFactory = new EnemyFactory(groupSpawnConfig.Enemy);

            var unitSize = groupSpawnConfig.Enemy.Prefab.transform.localScale;

            var count = 0;
            foreach (var spawnPoint in enemySpawnPoints)
            {
                int spawnCircleRadius = groupSpawnConfig.EnemyGroupsSpawnPoints[count].GroupCount * 2;
                for (int i = 0; i < groupSpawnConfig.EnemyGroupsSpawnPoints[count].GroupCount; i++)
                {
                    var unitSpawnPoint = GetEmptySpawnPoint(spawnPoint, unitSize, spawnCircleRadius);
                    var enemyController = _enemyFactory.CreateEnemy(unitSpawnPoint, _playerController);
                    AddController(enemyController);
                }
                count++;
            }
        }

        private static Vector3 GetEmptySpawnPoint(Vector3 spawnPoint, Vector3 unitSize, int spawnCircleRadius)
        {
            var unitSpawnPoint = spawnPoint + (Vector3)(Random.insideUnitCircle * spawnCircleRadius);
            float unitMaxSize = unitSize.MaxVector3CoordinateOnPlane();
            
            while (UnityHelper.IsAnyObjectAtPosition(unitSpawnPoint, unitMaxSize))
            {
                unitSpawnPoint = spawnPoint + (Vector3)(Random.insideUnitCircle * spawnCircleRadius);
            }

            return unitSpawnPoint;
        }
    }
}