using Abstracts;
using System;
using Utilities.Reactive.SubscriptionProperty;

namespace Gameplay.Enemy.Movement
{
    public sealed class EnemyInputController : BaseController
    {
        private readonly SubscribedProperty<float> _horizontalInput;
        private readonly SubscribedProperty<float> _verticalInput;
        
        public EnemyInputController()
        {
            _horizontalInput = new SubscribedProperty<float>(0.0f);
            _verticalInput = new SubscribedProperty<float>(0.0f);
        }

        public void Deconstruct(out SubscribedProperty<float> horizontalInput, out SubscribedProperty<float> verticalInput)
        {
            horizontalInput = _horizontalInput;
            verticalInput = _verticalInput;
        }

        public void Accelerate()
        {
            _verticalInput.Value = 1.0f;
        }
        
        public void Decelerate()
        {
            _verticalInput.Value = -1.0f;
        }
        
        public void HoldSpeed()
        {
            _verticalInput.Value = 0.0f;
        }
        
        public void TurnRight(float value = 1.0f)
        {
            _horizontalInput.Value = Math.Abs(value);
        }
        
        public void TurnLeft(float value = 1.0f)
        {
            _horizontalInput.Value = -Math.Abs(value);
        }
        
        public void StopTurning()
        {
            _horizontalInput.Value = 0.0f;
        }
    }
}