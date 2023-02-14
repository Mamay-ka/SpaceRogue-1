using Abstracts;
using System;
using UnityEngine;
using Utilities.ResourceManagement;

namespace UI.Game
{
    public sealed class GameUIController : BaseController
    {
        public static PlayerStatusBarView PlayerStatusBarView { get; private set; }
        public static PlayerSpeedometerView PlayerSpeedometerView { get; private set; }
        public static PlayerWeaponView PlayerWeaponView { get; private set; }
        public static Transform EnemyHealthBars { get; private set; }
        public static Transform GameEventIndicators { get; private set; }

        private readonly MainCanvasView _mainCanvasView;

        private Canvas _playerStatusBarCanvas;
        private Canvas _playerSpeedometerCanvas;
        private Canvas _playerWeaponCanvas;
        private Canvas _playerDestroyedCanvas;

        private PlayerStatusBarView _playerStatusBarView;
        private PlayerSpeedometerView _playerSpeedometerView;
        private PlayerWeaponView _playerWeaponView;
        private DestroyPlayerMessageView _playerDestroyedMessageView;

        private readonly ResourcePath _playerStatusBarCanvasPath = new(Constants.Prefabs.Canvas.Game.StatusBarCanvas);
        private readonly ResourcePath _playerSpeedometerCanvasPath = new(Constants.Prefabs.Canvas.Game.SpeedometerCanvas);
        private readonly ResourcePath _playerWeaponCanvasPath = new(Constants.Prefabs.Canvas.Game.WeaponCanvas);
        private readonly ResourcePath _playerDestroyedCanvasPath = new(Constants.Prefabs.Canvas.Game.DestroyPlayerCanvas);

        private Action _exitToMenu;

        public GameUIController(Canvas mainCanvas, Action exitToMenu)
        {
            _mainCanvasView = mainCanvas.GetComponent<MainCanvasView>();
            _exitToMenu = exitToMenu;

            EnemyHealthBars = _mainCanvasView.EnemyHealthBars;
            GameEventIndicators = _mainCanvasView.GameEventIndicators;

            AddPlayerStatusBar();
            AddPlayerSpeedometer();
            AddPlayerWeapon();
        }

        private void AddPlayerStatusBar()
        {
            _playerStatusBarCanvas = ResourceLoader.LoadPrefabAsChild<Canvas>(_playerStatusBarCanvasPath, _mainCanvasView.PlayerInfo);
            _playerStatusBarView = _playerStatusBarCanvas.GetComponent<PlayerStatusBarView>();
            PlayerStatusBarView = _playerStatusBarView;
            AddGameObject(_playerStatusBarCanvas.gameObject);
        }

        private void AddPlayerSpeedometer()
        {
            _playerSpeedometerCanvas = ResourceLoader.LoadPrefabAsChild<Canvas>(_playerSpeedometerCanvasPath, _mainCanvasView.PlayerInfo);
            _playerSpeedometerView = _playerSpeedometerCanvas.GetComponent<PlayerSpeedometerView>();
            PlayerSpeedometerView = _playerSpeedometerView;
            AddGameObject(_playerSpeedometerCanvas.gameObject);
        }

        private void AddPlayerWeapon()
        {
            _playerWeaponCanvas = ResourceLoader.LoadPrefabAsChild<Canvas>(_playerWeaponCanvasPath, _mainCanvasView.PlayerInfo);
            _playerWeaponView = _playerWeaponCanvas.GetComponent<PlayerWeaponView>();
            PlayerWeaponView = _playerWeaponView;
            AddGameObject(_playerWeaponCanvas.gameObject);
        }

        protected override void OnDispose()
        {
            PlayerStatusBarView = null;
            PlayerSpeedometerView = null;
            PlayerWeaponView = null;
            EnemyHealthBars = null;
            GameEventIndicators = null;
        }
            
        public void AddDestroyPlayerMessage()
        {
            _playerDestroyedCanvas = ResourceLoader.LoadPrefabAsChild<Canvas>(_playerDestroyedCanvasPath, _mainCanvasView.transform);
            _playerDestroyedMessageView = _playerDestroyedCanvas.GetComponent<DestroyPlayerMessageView>();
            _playerDestroyedMessageView.Init(_exitToMenu);
            AddGameObject(_playerDestroyedCanvas.gameObject);
        }
    }
}