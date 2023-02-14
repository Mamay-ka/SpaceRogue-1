using Abstracts;
using Gameplay.Health;
using Gameplay.Input;
using Gameplay.Mechanics.Timer;
using Gameplay.Movement;
using Gameplay.Player.FrontalGuns;
using Gameplay.Player.Inventory;
using Gameplay.Player.Movement;
using Scriptables;
using Scriptables.Health;
using Scriptables.Modules;
using System;
using System.Collections.Generic;
using UI.Game;
using UnityEngine;
using Utilities.Reactive.SubscriptionProperty;
using Utilities.ResourceManagement;

namespace Gameplay.Player
{
    public sealed class PlayerController : BaseController
    {
        public PlayerView View => _view;

        private readonly ResourcePath _configPath = new(Constants.Configs.Player.PlayerConfig);
        private readonly ResourcePath _viewPath = new(Constants.Prefabs.Gameplay.Player);
        private readonly ResourcePath _crosshairPrefabPath = new(Constants.Prefabs.Stuff.Crosshair);

        private readonly PlayerConfig _config;
        private readonly PlayerView _view;

        private readonly SubscribedProperty<Vector3> _mousePositionInput = new();
        private readonly SubscribedProperty<float> _verticalInput = new();
        private readonly SubscribedProperty<float> _horizontalInput = new();//!!!!!
        private readonly SubscribedProperty<bool> _primaryFireInput = new();
        private readonly SubscribedProperty<bool> _changeWeaponInput = new ();

        
        
       
        private const byte MaxCountOfPlayerSpawnTries = 10;
        private const float PlayerSpawnClearanceRadius = 40.0f;

        public event Action PlayerDestroyed = () => { };

        public PlayerController(Vector3 playerPosition)
        {
            _config = ResourceLoader.LoadObject<PlayerConfig>(_configPath);
            _view = LoadView<PlayerView>(_viewPath, playerPosition);
                                                                                               //!!!!!
            var inputController = new InputController(_mousePositionInput, _verticalInput, _horizontalInput, _primaryFireInput, _changeWeaponInput);
            AddController(inputController);

            var inventoryController = AddInventoryController(_config.Inventory);
            var movementController = AddMovementController(_config.Movement, _view);
           
            var frontalGunsController = AddFrontalGunsController(inventoryController.Turrets, _view);
            var healthController = AddHealthController(_config.HealthConfig, _config.ShieldConfig);
            AddCrosshair();
        }

        private HealthController AddHealthController(HealthConfig healthConfig, ShieldConfig shieldConfig)
        {
            var healthController = new HealthController(healthConfig, shieldConfig, GameUIController.PlayerStatusBarView, _view);
            healthController.SubscribeToOnDestroy(Dispose);
            healthController.SubscribeToOnDestroy(OnPlayerDestroyed);
            AddController(healthController);
            return healthController;
        }

        private PlayerInventoryController AddInventoryController(PlayerInventoryConfig config)
        {
            var inventoryController = new PlayerInventoryController(config);
            AddController(inventoryController);
            return inventoryController;
        }

        private PlayerMovementController AddMovementController(MovementConfig movementConfig, PlayerView view)
        {                                                                                                   //!!!!
            var movementController = new PlayerMovementController(_mousePositionInput, _verticalInput, _horizontalInput, movementConfig, view);
            AddController(movementController);
            return movementController;
        }

        private FrontalGunsController AddFrontalGunsController(List<TurretModuleConfig> turretConfigs, PlayerView view)
        {
            var frontalGunsController = new FrontalGunsController(_primaryFireInput, _changeWeaponInput, turretConfigs, view);
            AddController(frontalGunsController);
            return frontalGunsController;
        }

        private void AddCrosshair()
        {
            var crosshairView = ResourceLoader.LoadPrefab(_crosshairPrefabPath);
            var viewTransform = _view.transform;
            var crosshair = UnityEngine.Object.Instantiate(
                crosshairView,
                viewTransform.position + _view.transform.TransformDirection(Vector3.up * (viewTransform.localScale.y + 20f)),
                viewTransform.rotation
            );
            crosshair.transform.parent = _view.transform;
            AddGameObject(crosshair);
        }

        public void OnPlayerDestroyed()
        {
            PlayerDestroyed();
        }
    }
}