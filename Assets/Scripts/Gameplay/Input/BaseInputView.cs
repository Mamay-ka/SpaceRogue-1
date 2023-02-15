using UnityEngine;
using Utilities.Reactive.SubscriptionProperty;

namespace Gameplay.Input
{
    public abstract class BaseInputView : MonoBehaviour
    {
        private SubscribedProperty<Vector3> _mousePositionInput;
        
        private SubscribedProperty<float> _verticalAxisInput;
        private SubscribedProperty<float> _horizontalAxisInput;

        private SubscribedProperty<bool> _primaryFireInput;
        private SubscribedProperty<bool> _changeWeaponInput;
        private SubscribedProperty<bool> _nextLevelInput;
        private SubscribedProperty<bool> _mapInput;
        
        public virtual void Init(
            SubscribedProperty<Vector3> mousePositionInput,
            SubscribedProperty<float> verticalMove,
            SubscribedProperty<float> horizontalMove,
            SubscribedProperty<bool> primaryFireInput,
            SubscribedProperty<bool> changeWeaponInput,
            SubscribedProperty<bool> nextLevelInput,
            SubscribedProperty<bool> mapInput)
        {
            _mousePositionInput = mousePositionInput;
            _verticalAxisInput = verticalMove;
            _horizontalAxisInput = horizontalMove;
            _primaryFireInput = primaryFireInput;
            _changeWeaponInput = changeWeaponInput;
            _nextLevelInput = nextLevelInput;
            _mapInput = mapInput;
        }

        protected virtual void OnMousePositionInput(Vector3 value)
            => _mousePositionInput.Value = value;

        protected virtual void OnVerticalInput(float value) 
            => _verticalAxisInput.Value = value;

        protected virtual void OnHorizontalInput(float value)
            => _horizontalAxisInput.Value = value;

        protected virtual void OnPrimaryFireInput(bool value)
            => _primaryFireInput.Value = value;
        
        protected virtual void OnChangeWeaponInput(bool value)
            => _changeWeaponInput.Value = value;

        protected virtual void OnNextLevelInput(bool value)
            => _nextLevelInput.Value = value;
        
        protected virtual void OnMapInput(bool value)
            => _mapInput.Value = value;
    }
}