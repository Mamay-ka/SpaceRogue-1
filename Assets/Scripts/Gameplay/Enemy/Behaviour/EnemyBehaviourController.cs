using Abstracts;
using Gameplay.Enemy.Movement;
using Gameplay.Movement;
using Gameplay.Player;
using Gameplay.Shooting;
using UnityEngine;
using Utilities.Reactive.SubscriptionProperty;

namespace Gameplay.Enemy.Behaviour
{
    public sealed class EnemyBehaviourController : BaseController
    {
        private readonly EnemyView _view;
        private readonly MovementModel _movementModel;
        private readonly EnemyInputController _inputController;
        private readonly EnemyMovementController _movementController;
        private readonly FrontalTurretController _turretController;
        private readonly PlayerController _playerController;
        private readonly Transform _target;
        private readonly EnemyBehaviourConfig _enemyConfig;
        private readonly SubscribedProperty<EnemyState> _enemyCurrentState;
        
        
        private EnemyBehaviour _currentBehaviour;

        public EnemyBehaviourController(MovementModel movementModel, EnemyView view, FrontalTurretController turretController,
            PlayerController playerController, EnemyBehaviourConfig config, Transform target)
        {
            _view = view;
            _movementModel = movementModel;
            _inputController = AddInputController();
            _movementController = AddMovementController();

            _turretController = turretController;
            _playerController = playerController;
            _target = target;

            _enemyConfig = config;
            _enemyCurrentState = new(_enemyConfig.StartEnemyState);
            _enemyCurrentState.Subscribe(OnEnemyStateChange);
            OnEnemyStateChange(_enemyCurrentState.Value);
        }

        protected override void OnDispose()
        {
            _enemyCurrentState.Unsubscribe(OnEnemyStateChange);
            _currentBehaviour.Dispose();
        }

        private void OnEnemyStateChange(EnemyState newState)
        {
            var lastEnemyState = _currentBehaviour is null ? EnemyState.PassiveRoaming : _currentBehaviour.CurrentState();
            _currentBehaviour?.Dispose();
            
            switch (newState)
            {
                case EnemyState.Idle:
                    _currentBehaviour = new EnemyIdleBehaviour(_enemyCurrentState, _view, _playerController, _enemyConfig);
                    break;
                case EnemyState.PassiveRoaming:
                    _currentBehaviour = new EnemyRoamingBehaviour(_enemyCurrentState, _view, _playerController, _movementModel, _inputController, _enemyConfig);
                    break;
                case EnemyState.InCombat:
                    _currentBehaviour = new EnemyCombatBehaviour(_enemyCurrentState, _view, _playerController, _inputController, _turretController, _enemyConfig);
                    break;
                case EnemyState.InCombatWithRetreat:
                    _currentBehaviour = new EnemyCombatWithRetreatBehaviour(_enemyCurrentState, _view, _playerController, _inputController, _turretController, _enemyConfig, lastEnemyState);
                    break;
                case EnemyState.Escort:
                    _currentBehaviour = new EnemyEscortBehaviour(_enemyCurrentState, _view, _playerController, _inputController, _enemyConfig, _target);
                    break;
                default: return;
            }
        }

        private EnemyInputController AddInputController()
        {
            var inputController = new EnemyInputController();
            AddController(inputController);
            return inputController;
        }

        private EnemyMovementController AddMovementController()
        {
            var (horizontalInput, verticalInput) = _inputController;
            var movementController = new EnemyMovementController(horizontalInput, verticalInput, _movementModel, _view);
            AddController(movementController);
            return movementController;
        }
    }
}