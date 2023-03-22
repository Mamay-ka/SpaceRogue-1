using System;
using Gameplay.Events;
using Gameplay.Player.Movement;
using Gameplay.Player.Weapon;
using UnityEngine;
using Zenject;

namespace Gameplay.Player
{
    public sealed class PlayerFactory : PlaceholderFactory<Vector2, Player>
    {
        private readonly PlayerViewFactory _playerViewFactory;
        private readonly PlayerMovementFactory _playerMovementFactory;
        private readonly PlayerTurningFactory _playerTurningFactory;
        private readonly PlayerSurvivalFactory _playerSurvivalFactory;
        private readonly PlayerWeaponFactory _playerWeaponFactory;
        private readonly PlayerDashFactory _playerDashFactory;//!!!
        public event Action<PlayerSpawnedEventArgs> PlayerSpawned = _ => { };

        public PlayerFactory(
            PlayerViewFactory playerViewFactory, 
            PlayerMovementFactory playerMovementFactory, 
            PlayerTurningFactory playerTurningFactory,
            PlayerSurvivalFactory playerSurvivalFactory,
            PlayerWeaponFactory playerWeaponFactory,
            PlayerDashFactory playerDashFactory)//!!!
        {
            _playerViewFactory = playerViewFactory;
            _playerMovementFactory = playerMovementFactory;
            _playerTurningFactory = playerTurningFactory;
            _playerSurvivalFactory = playerSurvivalFactory;
            _playerWeaponFactory = playerWeaponFactory;
            _playerDashFactory = playerDashFactory;//!!!
        }

        public override Player Create(Vector2 spawnPoint)
        {
            var playerView = _playerViewFactory.Create(spawnPoint);
            var playerMovement = _playerMovementFactory.Create(playerView);
            var playerTurning = _playerTurningFactory.Create(playerView);
            var playerWeapon = _playerWeaponFactory.Create(playerView);
            var playerSurvival = _playerSurvivalFactory.Create();
            var playerDash = _playerDashFactory.Create(playerView);//!!!
            
            PlayerSpawned.Invoke(new PlayerSpawnedEventArgs
            {
                Transform = playerView.transform
            });
            
            return new Player(playerView, playerMovement, playerTurning, playerSurvival, playerWeapon, playerDash);//!!!
        }
    }
}