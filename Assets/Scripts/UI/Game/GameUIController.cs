using Abstracts;
using System;
using UnityEngine;
using Utilities.ResourceManagement;

namespace UI.Game
{
    public sealed class GameUIController : BaseController
    {
        public static Canvas MainCanvas { get; private set; }
        public static PlayerStatusBarView PlayerStatusBarView { get; private set; }
        public static PlayerSpeedometerView PlayerSpeedometerView { get; private set; }
        public static PlayerWeaponView PlayerWeaponView { get; private set; }
        public static LevelNumberView LevelNumberView { get; private set; }
        public static EnemiesCountView EnemiesCountView { get; private set; }
        public static Camera MinimapCamera { get; private set; }
        public static MinimapView MinimapView { get; private set; }
        public static Transform EnemyHealthBars { get; private set; }
        public static Transform GameEventIndicators { get; private set; }

        private GameCanvasView _gameCanvasView;
        private DestroyPlayerMessageView _playerDestroyedMessageView;
        private NextLevelMessageView _nextLevelMessageView;

        private readonly ResourcePath _gameCanvasPath = new(Constants.Prefabs.Canvas.Game.GameCanvas);
        private readonly ResourcePath _playerStatusBarCanvasPath = new(Constants.Prefabs.Canvas.Game.StatusBarCanvas);
        private readonly ResourcePath _playerSpeedometerCanvasPath = new(Constants.Prefabs.Canvas.Game.SpeedometerCanvas);
        private readonly ResourcePath _playerWeaponCanvasPath = new(Constants.Prefabs.Canvas.Game.WeaponCanvas);
        private readonly ResourcePath _levelNumberCanvasPath = new(Constants.Prefabs.Canvas.Game.LevelNumberCanvas);
        private readonly ResourcePath _enemiesCountCanvasPath = new(Constants.Prefabs.Canvas.Game.EnemiesCountCanvas);
        private readonly ResourcePath _playerDestroyedCanvasPath = new(Constants.Prefabs.Canvas.Game.DestroyPlayerCanvas);
        private readonly ResourcePath _nextLevelCanvasPath = new(Constants.Prefabs.Canvas.Game.NextLevelCanvas);
        private readonly ResourcePath _minimapCameraPath = new(Constants.Prefabs.Canvas.Game.MinimapCamera);
        private readonly ResourcePath _minimapCanvasPath = new(Constants.Prefabs.Canvas.Game.MinimapCanvas);

        private readonly Action _exitToMenu;
        private readonly Action _nextLevel;

        public GameUIController(Canvas mainCanvas, Action exitToMenu, Action nextLevel)
        {
            MainCanvas = mainCanvas;
            AddGameCanvas(MainCanvas.transform);
            _exitToMenu = exitToMenu;
            _nextLevel = nextLevel;

            EnemyHealthBars = _gameCanvasView.EnemyHealthBars;
            GameEventIndicators = _gameCanvasView.GameEventIndicators;

            AddPlayerStatusBar();
            AddPlayerSpeedometer();
            AddPlayerWeapon();
            AddLevelNumber();
            AddEnemiesCount();
            AddMinimap();
        }

        private void AddGameCanvas(Transform transform)
        {
            _gameCanvasView = ResourceLoader.LoadPrefabAsChild<GameCanvasView>(_gameCanvasPath, transform);
            AddGameObject(_gameCanvasView.gameObject);
        }

        private void AddPlayerStatusBar()
        {
            PlayerStatusBarView = ResourceLoader.LoadPrefabAsChild<PlayerStatusBarView>
                (_playerStatusBarCanvasPath, _gameCanvasView.PlayerInfo);
            AddGameObject(PlayerStatusBarView.gameObject);
        }

        private void AddPlayerSpeedometer()
        {
            PlayerSpeedometerView = ResourceLoader.LoadPrefabAsChild<PlayerSpeedometerView>
                (_playerSpeedometerCanvasPath, _gameCanvasView.PlayerInfo);
            AddGameObject(PlayerSpeedometerView.gameObject);
        }

        private void AddPlayerWeapon()
        {
            PlayerWeaponView = ResourceLoader.LoadPrefabAsChild<PlayerWeaponView>
                (_playerWeaponCanvasPath, _gameCanvasView.PlayerInfo);
            AddGameObject(PlayerWeaponView.gameObject);
        }

        private void AddLevelNumber()
        {
            LevelNumberView = ResourceLoader.LoadPrefabAsChild<LevelNumberView>
                (_levelNumberCanvasPath, _gameCanvasView.LevelInfo);
            AddGameObject(LevelNumberView.gameObject);
        }
        
        private void AddEnemiesCount()
        {
            EnemiesCountView = ResourceLoader.LoadPrefabAsChild<EnemiesCountView>
                (_enemiesCountCanvasPath, _gameCanvasView.LevelInfo);
            AddGameObject(EnemiesCountView.gameObject);
        }
        
        private void AddMinimap()
        {
            MinimapCamera = ResourceLoader.LoadPrefab<Camera>(_minimapCameraPath);
            AddGameObject(MinimapCamera.gameObject);

            MinimapView = ResourceLoader.LoadPrefabAsChild<MinimapView>
                (_minimapCanvasPath, _gameCanvasView.transform);
            AddGameObject(MinimapView.gameObject);
        }

        protected override void OnDispose()
        {
            MainCanvas = null;
            PlayerStatusBarView = null;
            PlayerSpeedometerView = null;
            PlayerWeaponView = null;
            LevelNumberView = null;
            EnemiesCountView = null;
            MinimapCamera = null;
            MinimapView = null;
            EnemyHealthBars = null;
            GameEventIndicators = null;
        }
            
        public void AddDestroyPlayerMessage(float levelsNumber)
        {
            _playerDestroyedMessageView = ResourceLoader.LoadPrefabAsChild<DestroyPlayerMessageView>
                (_playerDestroyedCanvasPath, _gameCanvasView.transform);
            _playerDestroyedMessageView.Init(levelsNumber, _exitToMenu);
            AddGameObject(_playerDestroyedMessageView.gameObject);
        }

        public void AddNextLevelMessage(float levelNumber)
        {
            _nextLevelMessageView = ResourceLoader.LoadPrefabAsChild<NextLevelMessageView>
                (_nextLevelCanvasPath, _gameCanvasView.transform);
            _nextLevelMessageView.Init(levelNumber, _nextLevel);
            AddGameObject(_nextLevelMessageView.gameObject);
        }
    }
}