using Abstracts;
using Gameplay.Background;
using Gameplay.Camera;
using Gameplay.Enemy;
using Gameplay.GameEvent;
using Gameplay.GameState;
using Gameplay.LevelProgress;
using Gameplay.Minimap;
using Gameplay.Player;
using Gameplay.Space;
using UI.Game;
using UnityEngine;

namespace Gameplay
{
    public sealed class GameController : BaseController
    {
        private readonly CurrentState _currentState;
        private readonly GameDataController _gameDataController;
        private readonly GameUIController _gameUIController;
        private readonly BackgroundController _backgroundController;
        private readonly SpaceController _spaceController;
        private readonly PlayerController _playerController;
        private readonly CameraController _cameraController;
        private readonly EnemyForcesController _enemyForcesController;
        private readonly GeneralGameEventsController _generalGameEventsController;
        private readonly LevelProgressController _levelProgressController;
        private readonly MinimapController _minimapController;

        public GameController(CurrentState currentState, Canvas mainUICanvas, GameDataController gameDataController)
        {
            _currentState = currentState;
            _gameDataController = gameDataController;

            _gameUIController = new(mainUICanvas, ExitToMenu, NextLevel);
            AddController(_gameUIController);

            _backgroundController = new();
            AddController(_backgroundController);

            _spaceController = new();
            AddController(_spaceController);

            _playerController = new(_spaceController.GetPlayerSpawnPoint(), _gameDataController.PlayerHealthInfo, _gameDataController.PlayerShieldInfo);
            AddController(_playerController);
            _playerController.PlayerDestroyed += OnPlayerDestroyed;

            _cameraController = new(_playerController);
            AddController(_cameraController);

            _enemyForcesController = new(_playerController, _spaceController.GetEnemySpawnPoints());
            AddController(_enemyForcesController);

            _generalGameEventsController = new(_playerController);
            AddController(_generalGameEventsController);

            _levelProgressController = new(_gameDataController, _playerController, _enemyForcesController.EnemyViews);
            _levelProgressController.LevelComplete += LevelComplete;
            AddController(_levelProgressController);

            _minimapController = new(_playerController, _spaceController.GetMapCameraSize());
            AddController(_minimapController);
        }

        private void OnPlayerDestroyed()
        {
            _gameUIController.AddDestroyPlayerMessage(_gameDataController.CompletedLevels);
        }

        private void LevelComplete(float levelNumber)
        {
            _levelProgressController.UpdatePlayerHealthAndShieldInfo
                (_playerController.GetCurrentHealth(), _playerController.GetCurrentShield());
            _gameUIController.AddNextLevelMessage(levelNumber);
        }

        public void ExitToMenu() 
        {
            _currentState.CurrentGameState.Value = GameState.GameState.Menu;
        }

        public void NextLevel()
        {
            _currentState.CurrentGameState.Value = GameState.GameState.Game;
        }
    }
}