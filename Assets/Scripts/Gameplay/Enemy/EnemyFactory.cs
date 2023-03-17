using Gameplay.Enemy.Scriptables;
using UnityEngine;
using Zenject;

namespace Gameplay.Enemy
{
    public sealed class EnemyFactory : PlaceholderFactory<Vector2, EnemyConfig, Enemy>
    {
        private readonly EnemyViewFactory _enemyViewFactory;
        private readonly EnemySurvivalFactory _enemySurvivalFactory;

        public EnemyFactory(EnemyViewFactory enemyViewFactory, EnemySurvivalFactory enemySurvivalFactory)
        {
            _enemyViewFactory = enemyViewFactory;
            _enemySurvivalFactory = enemySurvivalFactory;
        }

        public override Enemy Create(Vector2 spawnPoint, EnemyConfig enemyConfig)
        {
            var enemyView = _enemyViewFactory.Create(spawnPoint, enemyConfig);
            var enemySurvival = _enemySurvivalFactory.Create(enemyConfig.Survival);
            return new(enemyView, enemySurvival);
        }
    }
}