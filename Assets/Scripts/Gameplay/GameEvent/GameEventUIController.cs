using Abstracts;
using UnityEngine;
using Utilities.Reactive.SubscriptionProperty;
using Utilities.Unity;

namespace Gameplay.GameEvent
{
    public sealed class GameEventUIController : BaseController
    {
        private readonly Collider2D _gameEventObjectCollider;
        private readonly GameEventIndicatorView _indicatorView;
        private readonly UnityEngine.Camera _camera;

        private readonly SubscribedProperty<bool> _isVisible = new();
        
        private readonly bool _showUntilItIsVisibleOnce;
        private bool _isVisibleOnce;

        public GameEventUIController(GameEventIndicatorView indicatorView, Collider2D collider, 
            Sprite icon, float indicatorDiameter, bool showUntilItIsVisibleOnce = false)
        {
            _camera = UnityEngine.Camera.main;
            _indicatorView = indicatorView;
            AddGameObject(_indicatorView.gameObject);
            _indicatorView.gameObject.SetActive(false);
            _gameEventObjectCollider = collider;

            _indicatorView.Icon.sprite = icon;
            _indicatorView.IndicatorDiameter.sizeDelta = new(0, indicatorDiameter);

            _showUntilItIsVisibleOnce = showUntilItIsVisibleOnce;

            _isVisible.Subscribe(ShowIndicator);
            EntryPoint.SubscribeToUpdate(RotateToGameEventObject);
        }

        protected override void OnDispose()
        {
            _isVisible.Unsubscribe(ShowIndicator);
            EntryPoint.UnsubscribeFromUpdate(RotateToGameEventObject);
        }

        private void RotateToGameEventObject()
        {
            if (_gameEventObjectCollider == null)
            {
                Dispose();
                return;
            }

            _isVisible.Value = UnityHelper.IsObjectVisible(_camera, _gameEventObjectCollider.bounds);

            if (_isVisible.Value)
            {
                if (_showUntilItIsVisibleOnce)
                {
                    _isVisibleOnce = true;
                }
                return;
            }

            var position = _camera.WorldToScreenPoint(_gameEventObjectCollider.transform.position);
            position = new Vector3(position.x - Screen.width / 2, position.y - Screen.height / 2, 0);
            var angle = Mathf.Atan2(position.x, position.y) * Mathf.Rad2Deg;

            _indicatorView.transform.eulerAngles = Vector3.forward * -angle;
        }

        private void ShowIndicator(bool isVisible)
        {
            var value = default(bool);

            if (_isVisibleOnce)
            {
                value = false;
            }
            else
            {
                value = !isVisible;
            }

            _indicatorView.gameObject.SetActive(value);
        }
    }
}