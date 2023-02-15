using Abstracts;
using Gameplay.Enemy.Movement;
using Gameplay.Mechanics.Timer;
using Gameplay.Movement;
using Gameplay.Player;
using UnityEngine;
using Utilities.Reactive.SubscriptionProperty;
using Utilities.Unity;

namespace Gameplay.Enemy.Behaviour
{
    public sealed class EnemyRoamingBehaviour : EnemyBehaviour
    {
        private const float TurnValueAtObstacle = 0.1f;

        private readonly MovementModel _movementModel;
        private readonly EnemyInputController _inputController;
        private readonly Timer _timer;
        
        private Vector3 _targetDirection;
        private bool _frontObstacle;
        private bool _rightObstacle;
        private bool _leftObstacle;

        public EnemyRoamingBehaviour(
            SubscribedProperty<EnemyState> enemyState, EnemyView view, PlayerController playerController,
            MovementModel movementModel, EnemyInputController inputController, EnemyBehaviourConfig config) 
            : base(enemyState, view, playerController, config)
        {
            _movementModel = movementModel;
            _inputController = inputController;
            _timer = new(Config.TimeToPickNewAngle);
            PickRandomDirection();
        }
        
        protected override void OnUpdate()
        {
            CheckTimer();
            CheckObstacles();
            MoveAtLowSpeed(_frontObstacle);
            TurnToDirection(_rightObstacle, _leftObstacle);
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

        private void CheckObstacles()
        {
            var position = View.transform.position;
            var scaleY = View.transform.localScale.y;
            var scaleX = View.transform.localScale.x;

            var rayUpPosition = position + View.transform.TransformDirection(Vector3.up * scaleY);
            var rayRightPosition = position + View.transform.TransformDirection(Vector3.right * scaleX);
            var rayLeftPosition = position + View.transform.TransformDirection(Vector3.left * scaleX);

            var hitUp = Physics2D.Raycast(rayUpPosition, View.transform.TransformDirection(Vector3.up), 
                Config.FrontCheckDistance);
            var hitRight = Physics2D.Raycast(rayRightPosition, View.transform.TransformDirection(Vector3.right),
                Config.SideCheckDistance);
            var hitLeft = Physics2D.Raycast(rayLeftPosition, View.transform.TransformDirection(Vector3.left),
                Config.SideCheckDistance);

            _frontObstacle = hitUp.collider != null && !hitUp.collider.TryGetComponent<UnitView>(out _);
            _rightObstacle = hitRight.collider != null && !hitRight.collider.TryGetComponent<UnitView>(out _);
            _leftObstacle = hitLeft.collider != null && !hitLeft.collider.TryGetComponent<UnitView>(out _);
        }

        private void CheckTimer()
        {
            if (_timer.IsExpired)
            {
                _timer.Start();
                PickRandomDirection();
            }
        }

        private void PickRandomDirection()
        {
            _targetDirection = View.transform.TransformDirection(Random.insideUnitCircle).normalized;
        }

        private void MoveAtLowSpeed(bool frontObstacle)
        {
            if (frontObstacle)
            {
                _inputController.Decelerate();
                return;
            }

            var quarterMaxSpeed = _movementModel.MaxSpeed / 4;
            switch (CompareSpeeds(_movementModel.CurrentSpeed, quarterMaxSpeed))
            {
                case -1: { _inputController.Accelerate(); return; }
                case 0: { _inputController.HoldSpeed(); return; }
                case 1: { _inputController.Decelerate(); return; }
            }
        }

        private int CompareSpeeds(float currentSpeed, float targetSpeed)
        {
            if (UnityHelper.Approximately(currentSpeed, targetSpeed, 0.1f)) return 0;
            if (currentSpeed < targetSpeed) return -1;
            return 1;
        }

        private void TurnToDirection(bool rightObstacle, bool leftObstacle)
        {
            if (rightObstacle)
            {
                _inputController.TurnLeft(TurnValueAtObstacle);
            }

            if (leftObstacle)
            {
                _inputController.TurnRight(TurnValueAtObstacle);
            }

            if (rightObstacle || leftObstacle)
            {
                return;
            }

            var currentDirection = View.transform.TransformDirection(Vector3.up);

            if (UnityHelper.Approximately(_targetDirection, currentDirection, 0.1f))
            {
                _inputController.StopTurning();
            }
            else
            {
                HandleTurn(UnityHelper.VectorAngleLessThanAngle(_targetDirection, currentDirection, 0));
            }
            
        }

        private void HandleTurn(bool turningLeft)
        {
            if (turningLeft)
            {
                _inputController.TurnLeft();
            }
            else
            {
                _inputController.TurnRight();
            }
        }

        private void EnterCombat()
        {
            ChangeState(EnemyState.InCombat);
        }
    }
}