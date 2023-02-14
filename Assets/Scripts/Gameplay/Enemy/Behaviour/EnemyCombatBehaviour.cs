using Gameplay.Enemy.Movement;
using Gameplay.Player;
using Gameplay.Shooting;
using UnityEngine;
using Utilities.Reactive.SubscriptionProperty;
using Utilities.Unity;

namespace Gameplay.Enemy.Behaviour
{
    public sealed class EnemyCombatBehaviour : EnemyBehaviour
    {
        private readonly EnemyInputController _inputController;
        private readonly FrontalTurretController _frontalTurret;
        private readonly float _firingAngle;
        private EnemyState _lastEnemyState;

        private Vector3 _targetDirection;
        private Vector3 _currentDirection;
        private float _distance;
        private bool _inZone;

        public EnemyCombatBehaviour(
            SubscribedProperty<EnemyState> enemyState, 
            EnemyView view,
            PlayerController playerController,
            EnemyInputController inputController,
            FrontalTurretController frontalTurret,
            EnemyBehaviourConfig config,
            EnemyState lastEnemyState) : base(enemyState, view, playerController, config)
        {
            _inputController = inputController;
            _frontalTurret = frontalTurret;
            _firingAngle = config.FiringAngle;
            _lastEnemyState = lastEnemyState;
        }

        protected override void OnUpdate()
        {
            GetDirectionsAndDistance();
            RotateTowardsPlayer();
            Move();
            Shooting();
        }

        protected override void DetectPlayer()
        {
            if (_distance > Config.PlayerDetectionRadius)
            {
                _inZone = false;
                ExitCombat();
            }
            else
            {
                _inZone = true;
            }
        }

        private void GetDirectionsAndDistance()
        {
            _currentDirection = View.transform.TransformDirection(Vector3.up);
            var direction = PlayerView.transform.position - View.transform.position;
            _targetDirection = direction.normalized;
            _distance = direction.magnitude;
        }

        private void Shooting()
        {
            if (!_inZone)
            {
                return;
            }

            if (UnityHelper.DirectionInsideAngle(_targetDirection, _currentDirection, _firingAngle))
            {
                _frontalTurret.CommenceFiring();
            }
        }

        private void Move()
        {
            if (UnityHelper.Approximately(_distance, Config.ApproachDistance, 0.05f))
            {
                return;
            }

            if (_distance < Config.ApproachDistance)
            {
                _inputController.Decelerate();
            }
            else
            {
                _inputController.Accelerate();
            }
        }

        private void RotateTowardsPlayer()
        {
            if (_targetDirection == _currentDirection)
            {
                _inputController.StopTurning();
            }
            else
            {
                HandleTurn();
            }
        }

        private void HandleTurn()
        {
            if (UnityHelper.VectorAngleLessThanAngle(_targetDirection, _currentDirection, 0))
            {
                _inputController.TurnLeft();
            }
            else
            {
                _inputController.TurnRight();
            }
        }

        private void ExitCombat()
        {
            ChangeState(_lastEnemyState);
        }
    }
}