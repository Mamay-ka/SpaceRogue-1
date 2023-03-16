using Gameplay.Enemy.Scriptables;
using Gameplay.Enemy;
using Gameplay.Space.Generator;
using UnityEngine;
using Zenject;
using Gameplay.Pooling;

namespace Gameplay.Installers
{
    public sealed class EnemyInstaller : MonoInstaller
    {
        [field: SerializeField] public EnemyPool EnemyPool { get; private set; }
        [field: SerializeField] public EnemyGroupConfig EnemyGroupConfig { get; private set; }

        public override void InstallBindings()
        {
            InstallEnemyForces();
            InstallEnemy();
            InstallEnemyView();
        }

        private void InstallEnemyForces()
        {
            Container
                .Bind<EnemyGroupConfig>()
                .FromInstance(EnemyGroupConfig)
                .WhenInjectedInto<EnemyForcesFactory>();

            Container
                .BindFactory<int, SpawnPointsFinder, EnemyForces, EnemyForcesFactory>()
                .AsSingle();
        }

        private void InstallEnemy()
        {
            Container
                .BindFactory<Vector2, EnemyConfig, Enemy.Enemy, EnemyFactory>()
                .AsSingle();
        }

        private void InstallEnemyView()
        {
            Container
                .Bind<EnemyPool>()
                .FromInstance(EnemyPool)
                .WhenInjectedInto<EnemyViewFactory>();

            Container
                .BindFactory<Vector2, EnemyConfig, EnemyView, EnemyViewFactory>()
                .AsSingle();
        }
    }
}