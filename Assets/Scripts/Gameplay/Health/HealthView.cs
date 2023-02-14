using UI.Common;
using UnityEngine;

namespace Gameplay.Health
{
    public sealed class HealthView : MonoBehaviour
    {
        private SliderView _healthSliderView;
        private SliderView _shieldSliderView;

        public HealthView()
        {
            
        }

        public void InitHealth(float maxValue, float currentValue)
        {
            _healthSliderView.Init(0.0f, maxValue, currentValue);
        }

        public void InitShield(float maxValue, float currentValue)
        {
            _shieldSliderView.Init(0.0f, maxValue, currentValue);
        }

        public void UpdateHealthValue(float newValue)
        {
            _healthSliderView.UpdateValue(newValue);
        }
        
        public void UpdateShieldValue(float newValue)
        {
            _shieldSliderView.UpdateValue(newValue);
        }
    }
}