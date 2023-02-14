using System;

namespace Gameplay.Mechanics.Timer
{
    public sealed class Timer : IDisposable
    {
        public event Action OnStart = () => { };
        public event Action OnTick = () => { };
        public event Action OnExpire = () => { };

        public bool InProgress => _currentValue > 0.0f;
        public bool IsExpired => _currentValue == 0.0f;
        public float CurrentValue => _currentValue;

        private float _maxValue;
        private float _currentValue;
        
        public Timer(float value)
        {
            if (value == 0.0f) throw new ArgumentException("Timer cannot be initialized with zero!", nameof(value));
            
            _maxValue = value;
            _currentValue = 0.0f;

            EntryPoint.SubscribeToUpdate(Tick);
        }
        
        
        public void Dispose()
        {
            EntryPoint.UnsubscribeFromUpdate(Tick);
        }

        public void Start()
        {
            _currentValue = _maxValue;
            OnStart.Invoke();
        }

        public void SetMaxValue(float newMaxValue)
        {
            if (newMaxValue == 0.0f) throw new ArgumentException("Timer cannot be initialized with zero!", nameof(newMaxValue));
            
            float currentPercentage = _currentValue / _maxValue;
            float newCurrentValue = newMaxValue * currentPercentage;
            _maxValue = newMaxValue;
            _currentValue = newCurrentValue;
        }

        private void Tick(float deltaTime)
        {
            switch (_currentValue)
            {
                case 0:
                    return;
                case < 0:
                    _currentValue = 0;
                    OnExpire.Invoke();
                    return;
                case > 0:
                    _currentValue -= deltaTime;
                    OnTick.Invoke();
                    return;
            }
        }
    }
}