using Gameplay.Player;
using Scriptables.GameEvent;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Mathematics;
using Utilities.Unity;

namespace Gameplay.GameEvent
{
    public sealed class CometGameEventController : GameEventController
    {
        private const int MaxCountOfCometSpawnTries = 10;

        private readonly CometGameEventConfig _cometGameEventConfig;
        private readonly PlayerView _playerView;
        private readonly CometFactory _factory;
        private readonly float _orthographicSize;

        private readonly List<CometController> _cometControllers = new();
        private bool _isStopped;

        public CometGameEventController(GameEventConfig config, PlayerController playerController) : base(config, playerController)
        {
            var cometGameEventConfig = config as CometGameEventConfig;
            _cometGameEventConfig = cometGameEventConfig
                ? cometGameEventConfig
                : throw new System.Exception("Wrong config type was provided");

            _playerView = _playerController.View;
            _factory = new(_cometGameEventConfig.CometConfig);
            _orthographicSize = UnityEngine.Camera.main.orthographicSize;
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            EntryPoint.UnsubscribeFromUpdate(WaitAllLiveControllers);
        }

        protected override bool RunGameEvent()
        {
            if (_isStopped)
            {
                return true;
            }

            for (int i = 0; i < _cometControllers.Count; i++)
            {
                if (_cometControllers[i].IsDestroyed)
                {
                    _cometControllers.Remove(_cometControllers[i]);
                }
            }

            for (int i = 0; i < _cometGameEventConfig.CometConfig.CometCount; i++)
            {
                if (!TryGetNewCometPosition(out var position))
                {
                    Debug("No place for Comet");
                    continue;
                }
                var direction = _playerView.transform.position - position;
                var controller = _factory.CreateComet(position, direction);
                AddController(controller);
                AddGameEventObjectToUIController(controller.View.gameObject, true);
                _cometControllers.Add(controller);
            }

            if(_cometControllers.Count == 0)
            {
                return false;
            }
            return true;
        }

        protected override void OnPlayerDestroyed()
        {
            _isStopped = true;
            EntryPoint.SubscribeToUpdate(WaitAllLiveControllers);
        }

        private void WaitAllLiveControllers()
        {
            if(_cometControllers.Count == 0)
            {
                Dispose();
            }

            for (int i = 0; i < _cometControllers.Count; i++)
            {
                if (_cometControllers[i].IsDestroyed)
                {
                    _cometControllers.Remove(_cometControllers[i]);
                }
            }
        }

        private bool TryGetNewCometPosition(out Vector3 position)
        {
            var tryCount = 0;
            var radius = _cometGameEventConfig.CometConfig.Size + _cometGameEventConfig.CometConfig.CheckRadius;
            do
            {
                position = GetRandomCometPosition();
                tryCount++;
            }
            while (UnityHelper.IsAnyObjectAtPosition(position, radius) && tryCount <= MaxCountOfCometSpawnTries);

            if (tryCount > MaxCountOfCometSpawnTries)
            {
                return false;
            }

            return true;
        }

        private Vector3 GetRandomCometPosition()
        {
            var angleDirection = RandomPicker.PickRandomAngle(360, _random).normalized;
            var playerPosition = _playerView.transform.position;
            var offset = _orthographicSize * 2 + _cometGameEventConfig.CometConfig.Size + _cometGameEventConfig.CometConfig.Offset;
            var position = playerPosition + angleDirection * offset;
            return position;
        }
    }
}