using Abstracts;
using Gameplay.Movement;
using UI.Game;
using UnityEngine;
using Utilities.Reactive.SubscriptionProperty;
using Utilities.Unity;
using Gameplay.Mechanics.Timer;

namespace Gameplay.Player.Movement
{
    public sealed class PlayerMovementController : BaseController
    {
        private readonly SubscribedProperty<Vector3> _mousePositionInput;
        private readonly SubscribedProperty<float> _verticalInput;
        private readonly SubscribedProperty<float> _horizontalInput;

        private readonly PlayerSpeedometerView _speedometerView;
        private readonly MovementModel _model;
        private readonly PlayerView _view;
        private readonly Rigidbody2D _rigidbody;

        private Timer _cooldownDashTimer;
        
        private Vector3 _currentDirection;
        private float _lastTurnRate;
        
        public PlayerMovementController(
            SubscribedProperty<Vector3> mousePositionInput,
            SubscribedProperty<float> verticalInput,
            SubscribedProperty<float> horizontalInput,
            MovementConfig config,
            PlayerView view)
        {
            _mousePositionInput = mousePositionInput;
            _verticalInput = verticalInput;
            _horizontalInput = horizontalInput;
            _view = view;
            _rigidbody = _view.GetComponent<Rigidbody2D>();
            _model = new MovementModel(config);
            _speedometerView = GameUIController.PlayerSpeedometerView;
            _speedometerView.Init(GetSpeedometerTextValue(0.0f, _model.MaxSpeed));

            _cooldownDashTimer = new Timer(_model.DashCooldown);

            _mousePositionInput.Subscribe(HandleHorizontalMouseInput);
            _verticalInput.Subscribe(HandleVerticalInput);
            _horizontalInput.Subscribe(HandleHorizontalInput);
        }

        protected override void OnDispose()
        {
            _mousePositionInput.Unsubscribe(HandleHorizontalMouseInput);
            _verticalInput.Unsubscribe(HandleVerticalInput);
            _horizontalInput.Unsubscribe(HandleHorizontalInput);
            _cooldownDashTimer.Dispose();
        }

        private void HandleHorizontalInput(float newInputValue)
        {
            if (newInputValue < 0 && !_cooldownDashTimer.InProgress)
            {
                Dash(Vector3.left) ;
            }
            else if (newInputValue > 0 && !_cooldownDashTimer.InProgress)
            {
                Dash(Vector3.right);
            }
        }

        private void HandleVerticalInput(float newInputValue)
        {
            if (newInputValue != 0)
            {
                _model.Accelerate(newInputValue > 0);
            }

            float currentSpeed = _model.CurrentSpeed;
            float maxSpeed = _model.MaxSpeed;
            UpdateSpeedometerValue(currentSpeed, maxSpeed);
            
            if (currentSpeed != 0)
            {
                var transform = _view.transform;
                var forwardDirection = transform.TransformDirection(Vector3.up);
                
                _rigidbody.AddForce(forwardDirection.normalized * currentSpeed, ForceMode2D.Force);
            }
            
            if (_rigidbody.velocity.sqrMagnitude > maxSpeed * maxSpeed)
            {
                Vector3 velocity = _rigidbody.velocity.normalized * maxSpeed;
                _rigidbody.velocity = velocity;
            }

            if (newInputValue == 0 && currentSpeed < _model.StoppingSpeed && currentSpeed > -_model.StoppingSpeed)
            {
                _model.StopMoving();
            }
        }

        private void HandleHorizontalMouseInput(Vector3 newMousePositionInput)
        {
            var mousePosition = UnityEngine.Camera.main.ScreenToWorldPoint(newMousePositionInput);
            mousePosition.z = 0;

            var transform = _view.transform;
            var direction = (mousePosition - transform.position).normalized;
            _currentDirection = transform.TransformDirection(Vector3.up);
            float angle = Vector2.SignedAngle(direction, _currentDirection);

            Quaternion newRotation;
            if (UnityHelper.Approximately(angle, 0, Mathf.Abs(_lastTurnRate)))
            {
                _model.StopTurning();
                _lastTurnRate = _model.StartingTurnSpeed;

                if (angle > 0)
                {
                    newRotation = GetNewRotation(transform, -angle, Vector3.forward);
                }
                else
                {
                    newRotation = GetNewRotation(transform, angle, Vector3.back);
                }
                _rigidbody.MoveRotation(newRotation);
                _rigidbody.angularVelocity = 0;
                return;
            }

            if (angle > 0)
            {
                _model.Turn(true);
                newRotation = GetNewRotation(transform, _model.CurrentTurnRate, Vector3.forward);
            }
            else
            {
                _model.Turn(false);
                newRotation = GetNewRotation(transform, -_model.CurrentTurnRate, Vector3.back);
            }

            _lastTurnRate = _model.CurrentTurnRate;
            _rigidbody.MoveRotation(newRotation);
        }

        private Quaternion GetNewRotation(Transform transform, float angle, Vector3 axis)
        {
            return transform.rotation * Quaternion.AngleAxis(angle, axis);
        }

        private void UpdateSpeedometerValue(float currentSpeed, float maxSpeed)
        {
            _speedometerView.UpdateText(GetSpeedometerTextValue(currentSpeed, maxSpeed));
        }

        private static string GetSpeedometerTextValue(float currentSpeed, float maximumSpeed) =>
            currentSpeed switch
            {
                < 0 => "R",
                _ => $"SPD: {Mathf.RoundToInt(currentSpeed / maximumSpeed * 100)}"
            };

        private void Dash(Vector3 vector3)
        {
            var transform = _view.transform;
            var sideDirection = transform.TransformDirection(vector3);
            _rigidbody.AddForce(sideDirection.normalized * _model.DashLength * MovementModel.DashLengthMultiplier, ForceMode2D.Force);

            _cooldownDashTimer.Start();
        }
    }
}