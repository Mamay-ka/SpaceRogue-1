using System;
using Gameplay.Input;
using Gameplay.Mechanics.Timer;
using Gameplay.Movement;
using UnityEngine;

namespace Gameplay.Player.Movement
{

    public sealed class PlayerDash : IDisposable
    {
        private readonly PlayerInput _playerInput;
        //private readonly UnitMovementModel _model;
        private readonly Rigidbody2D _rigidbody;
        private readonly Transform _transform;
        private readonly UnitMovementConfig _config;

        public const float DashLengthMultiplier = 10000f;
        public Timer CooldownDashTimer { get; private set; }

        //public float DashLength => _config.dashLength;
        //public float DashCooldown => _config.dashCooldown;

        public PlayerDash(PlayerInput input, UnitMovementConfig movementConfig,
            PlayerView playerView, TimerFactory timerFactory)
        {
            _playerInput = input;
            //_model = movementModelFactory.Create(movementConfig);
            _rigidbody = playerView.GetComponent<Rigidbody2D>();
            _transform = playerView.transform;
            _config = movementConfig;
            CooldownDashTimer = timerFactory.Create(_config.dashCooldown);

            _playerInput.HorizontalAxisInput += HandleHorizontalInput;
        }

        public void Dispose()
        {
            _playerInput.HorizontalAxisInput -= HandleHorizontalInput;
        }

        private void HandleHorizontalInput(float newInputValue)
        {
            if (newInputValue < 0 && !CooldownDashTimer.InProgress)
            {
                Dash(Vector3.left);
            }
            else if (newInputValue > 0 && !CooldownDashTimer.InProgress)
            {
                Dash(Vector3.right);
            }
        }

        private void Dash(Vector3 vector3)
        {
            var transform = _transform.transform;
            var sideDirection = transform.TransformDirection(vector3);
            _rigidbody.AddForce(sideDirection.normalized * _config.dashLength * DashLengthMultiplier, ForceMode2D.Force);

            CooldownDashTimer.Start();
        }
    }
}
