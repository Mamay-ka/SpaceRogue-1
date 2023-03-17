using System;
using Gameplay.Survival;

namespace Gameplay.Enemy
{
    public sealed class Enemy : IDisposable
    {
        public EnemyView EnemyView { get; }
        public EntitySurvival Survival { get; }

        public Enemy(EnemyView enemyView, EntitySurvival enemySurvival)
        {
            EnemyView = enemyView;
            Survival = enemySurvival;
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(EnemyView.gameObject);
        }
    }
}