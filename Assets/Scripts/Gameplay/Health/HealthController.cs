using System;
using Abstracts;
using Gameplay.Damage;
using Scriptables.Health;
using UI.Game;

namespace Gameplay.Health
{
    public sealed class HealthController : BaseController
    {
        private readonly HealthStatusBarView _statusBarView;
        private readonly BaseHealthModel _healthModel;
        private readonly IDamageableView _damageable;
        
        private Action _onDestroy;

        public HealthStatusBarView StatusBarView => _statusBarView;

        public HealthController(HealthConfig healthConfig, ShieldConfig shieldConfig, HealthShieldStatusBarView statusBarView, IDamageableView damageable)
        {
            var healthModel = new HealthWithShieldModel(healthConfig, shieldConfig);
            
            statusBarView.HealthBar.Init(0.0f, healthModel.MaximumHealth.Value, healthModel.CurrentHealth.Value);
            statusBarView.ShieldBar.Init(0.0f, healthModel.MaximumShield.Value, healthModel.CurrentShield.Value);
            
            healthModel.CurrentHealth.Subscribe(statusBarView.HealthBar.UpdateValue);
            healthModel.CurrentShield.Subscribe(statusBarView.ShieldBar.UpdateValue);
            EntryPoint.SubscribeToUpdate(healthModel.UpdateState);

            damageable.DamageTaken += TakeDamage;
            _damageable = damageable;

            _statusBarView = statusBarView;
            _healthModel = healthModel;
        }
        
        public HealthController(HealthConfig healthConfig, ShieldConfig shieldConfig, IDamageableView damageable)
        {
            var healthModel = new HealthWithShieldModel(healthConfig, shieldConfig);
            
            EntryPoint.SubscribeToUpdate(healthModel.UpdateState);
            
            damageable.DamageTaken += TakeDamage;
            _damageable = damageable;
            
            _healthModel = healthModel;
        }

        public HealthController(HealthConfig healthConfig, HealthStatusBarView statusBarView, IDamageableView damageable)
        {
            var healthModel = new HealthOnlyModel(healthConfig);
            statusBarView.HealthBar.Init(0.0f, healthModel.MaximumHealth.Value, healthModel.CurrentHealth.Value);
            _statusBarView = statusBarView;

            damageable.DamageTaken += TakeDamage;
            _damageable = damageable;
            
            healthModel.CurrentHealth.Subscribe(statusBarView.HealthBar.UpdateValue);
            _healthModel = healthModel;
        }
        
        public HealthController(HealthConfig healthConfig, IDamageableView damageable)
        {
            var healthModel = new HealthOnlyModel(healthConfig);
            
            damageable.DamageTaken += TakeDamage;
            _damageable = damageable;
            
            EntryPoint.SubscribeToUpdate(healthModel.UpdateState);
            _healthModel = healthModel;
        }

        public void SubscribeToOnDestroy(Action onDestroyAction)
        {
            _onDestroy += onDestroyAction;
            _healthModel.UnitDestroyed += onDestroyAction;
        }

        protected override void OnDispose()
        {
            _damageable.DamageTaken -= TakeDamage;
            _healthModel.UnitDestroyed -= _onDestroy;
            EntryPoint.UnsubscribeFromUpdate(_healthModel.UpdateState);
            
            if (_statusBarView is not null) 
                _healthModel.CurrentHealth.Unsubscribe(_statusBarView.HealthBar.UpdateValue);

            if (_healthModel is HealthWithShieldModel healthShieldModel && _statusBarView is HealthShieldStatusBarView statusShieldBar) 
                healthShieldModel.CurrentShield.Unsubscribe(statusShieldBar.ShieldBar.UpdateValue);

        }

        private void TakeDamage(DamageModel damageModel)
        {
            if(_damageable is UnitView view && view.UnitType == damageModel.UnitType)
            {
                return;
            }

            if(damageModel.UnitType == UnitType.Assistant)
            {
                _healthModel.TakeHealth(damageModel.DamageAmount);
                return;
            }

            _healthModel.TakeDamage(damageModel.DamageAmount);
        }
    }
}