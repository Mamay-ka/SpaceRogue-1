using Gameplay.Enemy.Movement;
using Gameplay.Player;
using UnityEngine;
using Utilities.Reactive.SubscriptionProperty;
using Utilities.Unity;

namespace Gameplay.Enemy.Behaviour
{
    public sealed class EnemyEscortBehaviour : EnemyBehaviour
    {
        private readonly EnemyInputController _inputController;
        private readonly Transform _target;
        private Vector3 _targetDirection;
        private Vector3 _currentDirection;
        private float _distance;

        public EnemyEscortBehaviour(
            SubscribedProperty<EnemyState> enemyState, 
            EnemyView view,
            PlayerController playerController,
            EnemyInputController inputController,
            EnemyBehaviourConfig config,
            Transform target) : base(enemyState, view, playerController, config)
        {
            _inputController = inputController;
            _target = target;
        }

        protected override void OnUpdate()
        {
            if(_target == null)
            {
                ChangeState(EnemyState.PassiveRoaming);
                return;
            }

            GetDirectionsAndDistance();
            RotateTowardsTarget();
            Move();
        }

        protected override void DetectPlayer()
        {
            if (PlayerView == null)
            {
                return;
            }

            if (Vector3.Distance(View.transform.position, PlayerView.transform.position) < Config.PlayerDetectionRadius)
            {
                EnterCombat();
            }
        }

        private void GetDirectionsAndDistance()
        {
            _currentDirection = View.transform.TransformDirection(Vector3.up);
            var direction = _target.transform.position - View.transform.position;
            _targetDirection = direction.normalized;
            _distance = direction.magnitude;
        }

        private void RotateTowardsTarget()
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

        private void EnterCombat()
        {
            ChangeState(EnemyState.InCombatWithRetreat);
        }
    }
}