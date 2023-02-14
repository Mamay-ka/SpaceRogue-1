using Scriptables.Health;
using UnityEngine;
using Utilities.Reactive.SubscriptionProperty;

namespace Gameplay.Health
{
    public sealed class HealthWithShieldModel : HealthOnlyModel
    {
        public SubscribedProperty<float> CurrentShield { get; }
        public SubscribedProperty<float> MaximumShield { get; }

        private float _currentShieldCooldown;
        private readonly float _shieldCooldown;

        public HealthWithShieldModel(HealthConfig healthConfig, ShieldConfig shieldConfig) : base(healthConfig)
        {
            MaximumShield = new SubscribedProperty<float>(shieldConfig.ShieldAmount);
            CurrentShield = new SubscribedProperty<float>(shieldConfig.ShieldAmount);
            _shieldCooldown = shieldConfig.Cooldown;
            _currentShieldCooldown = 0f;
        }

        internal override void TakeDamage(float damageAmount)
        {
            if (CurrentDamageImmunityTime > 0.0f) return;

            StartDamageImmunityWindow();
            StartShieldCooldown();
            OnDamageTaken();

            if (CurrentShield.Value > 0)
            {
                if (CurrentShield.Value < damageAmount)
                {
                    CurrentShield.Value = 0;
                    TakeHealthDamage(damageAmount - CurrentShield.Value);
                }
                else
                {
                    TakeShieldDamage(damageAmount);
                }
                return;
            }
            
            TakeHealthDamage(damageAmount);
        }

        internal override void UpdateState()
        {
            if (CurrentDamageImmunityTime >= 0.0f) CooldownDamageImmunityFrame();
            
            if (_currentShieldCooldown <= 0.0f && CurrentShield.Value < MaximumShield.Value)
            {
                RefreshShield();
            }
            
            if (CurrentHealth.Value < MaximumHealth.Value)
            {
                CurrentHealth.Value += RegenAmountPerDeltaTime;
            }

            if (_currentShieldCooldown > 0.0f)
            {
                CooldownShield();
            }
        }

        private void TakeShieldDamage(float damageAmount)
        {
            CurrentShield.Value -= damageAmount;
        }

        private void StartShieldCooldown()
        {
            _currentShieldCooldown = _shieldCooldown;
        }

        private void CooldownShield()
        {
            _currentShieldCooldown -= Time.deltaTime;
        }

        private void RefreshShield()
        {
            CurrentShield.Value = MaximumShield.Value;
        }
    }
}