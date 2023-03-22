using System;
using Gameplay.Input;
using Gameplay.Mechanics.Timer;
using Gameplay.Movement;
using Services;
using UnityEngine;

namespace Gameplay.Player.Movement
{
    public sealed class PlayerMovement : IDisposable
    {
        private readonly PlayerInput _playerInput;
        private readonly UnitMovementModel _model;
        private readonly Rigidbody2D _rigidbody;
        private readonly Transform _transform;

        //public Timer CooldownDashTimer { get; private set; }
        
        public float CurrentSpeed => _model.CurrentSpeed;
        public float MaxSpeed => _model.MaxSpeed;
               
        public PlayerMovement(PlayerView playerView, PlayerInput playerInput,
            UnitMovementModelFactory movementModelFactory, UnitMovementConfig movementConfig, TimerFactory timerFactory)
        {
            _playerInput = playerInput;
            _model = movementModelFactory.Create(movementConfig);
            _rigidbody = playerView.GetComponent<Rigidbody2D>();
            _transform = playerView.transform;
            //CooldownDashTimer = timerFactory.Create(_model.DashCooldown);

            _playerInput.VerticalAxisInput += HandleVerticalInput;
            //_playerInput.HorizontalAxisInput += HandleHorizontalInput;
        }

        public void Dispose()
        {
            _playerInput.VerticalAxisInput -= HandleVerticalInput;
            //_playerInput.HorizontalAxisInput -= HandleHorizontalInput;
        }

        /*private void HandleHorizontalInput(float newInputValue)
        {
            if (newInputValue < 0 && !CooldownDashTimer.InProgress)
            {
                Dash(Vector3.left);
            }
            else if (newInputValue > 0 && !CooldownDashTimer.InProgress)
            {
                Dash(Vector3.right);
            }
        }*/
        private void HandleVerticalInput(float newInputValue)
        {
            if (newInputValue != 0)
            {
                _model.Accelerate(newInputValue > 0);
            }

            if (CurrentSpeed != 0)
            {
                var forwardDirection = _transform.TransformDirection(Vector3.up);
                
                _rigidbody.AddForce(forwardDirection.normalized * CurrentSpeed, ForceMode2D.Force);
            }
            
            if (_rigidbody.velocity.sqrMagnitude > MaxSpeed * MaxSpeed)
            {
                Vector3 velocity = _rigidbody.velocity.normalized * MaxSpeed;
                _rigidbody.velocity = velocity;
            }

            if (newInputValue == 0 && CurrentSpeed < _model.StoppingSpeed && CurrentSpeed > -_model.StoppingSpeed)
            {
                _model.StopMoving();
            }
        }

        /*private void Dash(Vector3 vector3)
        {
            var transform = _transform.transform;
            var sideDirection = transform.TransformDirection(vector3);
            _rigidbody.AddForce(sideDirection.normalized * _model.DashLength * UnitMovementModel.DashLengthMultiplier, ForceMode2D.Force);

           CooldownDashTimer.Start();
        }*/
    }
}