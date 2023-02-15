using Abstracts;
using Gameplay.Enemy;
using Gameplay.Health;
using Gameplay.Movement;
using Gameplay.Player;
using Scriptables.GameEvent;
using Scriptables.Health;
using UI.Game;
using UnityEngine;
using Utilities.Mathematics;
using Utilities.Reactive.SubscriptionProperty;
using Utilities.ResourceManagement;
using Utilities.Unity;

namespace Gameplay.GameEvent
{
    public sealed class CaravanController : BaseController
    {
        private const byte MaxCountSpawnTries = 10;

        private readonly BaseCaravanGameEventConfig _baseCaravanGameEvent;
        private readonly PlayerController _playerController;
        private readonly PlayerView _playerView;
        private readonly CaravanView _caravanView;

        private readonly ResourcePath _enemyHealthStatusBarCanvasPath =
            new(Constants.Prefabs.Canvas.Game.EnemyHealthStatusBarCanvas);
        private readonly ResourcePath _enemyHealthShieldStatusBarCanvasPath =
            new(Constants.Prefabs.Canvas.Game.EnemyHealthShieldStatusBarCanvas);

        public SubscribedProperty<bool> OnDestroy = new();

        public CaravanController(GameEventConfig config, PlayerController playerController, 
            CaravanView caravanView, Vector3 targetPosition)
        {
            var baseCaravanGameEvent = config as BaseCaravanGameEventConfig;
            _baseCaravanGameEvent = baseCaravanGameEvent
                ? baseCaravanGameEvent
                : throw new System.Exception("Wrong config type was provided");

            _playerController = playerController;
            _playerView = _playerController.View;

            _caravanView = caravanView;
            _caravanView.Init(new(0));
            AddGameObject(_caravanView.gameObject);

            AddCarnavalBehaviourController(_baseCaravanGameEvent.CaravanConfig.Movement, targetPosition);
            AddCaravanHealthUIController(_baseCaravanGameEvent.CaravanConfig.Health, _baseCaravanGameEvent.CaravanConfig.Shield);

            AddEnemyGroup(_baseCaravanGameEvent, _caravanView.transform.position, _playerController, _caravanView.transform);
        }

        private void AddCarnavalBehaviourController(MovementConfig movement, Vector3 targetPosition)
        {
            var behaviourController = new CaravanBehaviourController(new MovementModel(movement), _caravanView, targetPosition);
            AddController(behaviourController);
        }

        private EnemyHealthUIController AddCaravanHealthUIController(HealthConfig healthConfig, ShieldConfig shieldConfig)
        {
            var healthController = shieldConfig is null
                ? new HealthController(healthConfig,
                AddHealthStatusBarView(GameUIController.EnemyHealthBars), _caravanView)
                : new HealthController(healthConfig, shieldConfig,
                AddHealthShieldStatusBarView(GameUIController.EnemyHealthBars), _caravanView);

            healthController.SubscribeToOnDestroy(Dispose);
            healthController.SubscribeToOnDestroy(OnCaravanDestroyed);
            AddController(healthController);

            var enemyHealthUIController = new EnemyHealthUIController(healthController, _caravanView);
            AddController(enemyHealthUIController);
            return enemyHealthUIController;
        }

        private void OnCaravanDestroyed()
        {
            if (_playerView == null)
            {
                OnDestroy.Value = true;
                return;
            }

            if (_caravanView.IsLastDamageFromPlayer)
            {
                StandardCaravanDestroyed();
                CaravanTrapDestroyed();
            }

            OnDestroy.Value = true;
        }

        private void StandardCaravanDestroyed()
        {
            var config = _baseCaravanGameEvent as CaravanGameEventConfig;
            
            if(config == null)
            {
                return;
            }

            _caravanView.Init(new(config.AddHealth, UnitType.Assistant));
            _playerView.TakeDamage(_caravanView);
        }

        private void CaravanTrapDestroyed()
        {
            var config = _baseCaravanGameEvent as CaravanTrapGameEventConfig;

            if (config == null)
            {
                return;
            }

            Debug($"AlertRadius = {config.AlertRadius}");
        }

        private HealthStatusBarView AddHealthStatusBarView(Transform transform)
        {
            var enemyStatusBarView = ResourceLoader.LoadPrefabAsChild<HealthStatusBarView>
                (_enemyHealthStatusBarCanvasPath, transform);
            AddGameObject(enemyStatusBarView.gameObject);
            return enemyStatusBarView;
        }

        private HealthShieldStatusBarView AddHealthShieldStatusBarView(Transform transform)
        {
            var enemyStatusBarView = ResourceLoader.LoadPrefabAsChild<HealthShieldStatusBarView>(_enemyHealthShieldStatusBarCanvasPath, transform);
            AddGameObject(enemyStatusBarView.gameObject);
            return enemyStatusBarView;
        }

        private void AddEnemyGroup(BaseCaravanGameEventConfig config, Vector3 spawnPoint, 
            PlayerController playerController, Transform target)
        {
            var enemyFactory = new EnemyFactory(config.EnemyConfig);
            var unitSize = config.EnemyConfig.Prefab.transform.localScale;
            
            var spawnCircleRadius = config.EnemyCount * 2;
            for (int i = 0; i < config.EnemyCount; i++)
            {
                var unitSpawnPoint = GetEmptySpawnPoint(spawnPoint, unitSize, spawnCircleRadius);
                var enemyController = enemyFactory.CreateEnemy(unitSpawnPoint, playerController, target);
                AddController(enemyController);
            }
        }

        private Vector3 GetEmptySpawnPoint(Vector3 spawnPoint, Vector3 unitSize, int spawnCircleRadius)
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