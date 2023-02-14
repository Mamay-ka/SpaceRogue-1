using System;
using Scriptables.Health;
using UnityEngine;
using Utilities.Reactive.SubscriptionProperty;

namespace Gameplay.Health
{
    public abstract class BaseHealthModel
    {
        public event Action UnitDestroyed = () => { };
        public event Action DamageTaken = () => { };

        public SubscribedProperty<float> CurrentHealth { get; }
        public SubscribedProperty<float> MaximumHealth { get; }

        protected float CurrentDamageImmunityTime;
        protected readonly float HealthRegenAmount;
        protected readonly float DamageImmunityFrameDuration;

        protected float RegenAmountPerDeltaTime => HealthRegenAmount * Time.deltaTime;

        internal BaseHealthModel(HealthConfig healthConfig)
        {
            MaximumHealth = new SubscribedProperty<float>(healthConfig.MaximumHealth);
            var correctHealth = Mathf.Clamp(healthConfig.StartingHealth, 0f, healthConfig.MaximumHealth);
            CurrentHealth = new SubscribedProperty<float>(correctHealth);
            HealthRegenAmount = healthConfig.HealthRegen;
            DamageImmunityFrameDuration = healthConfig.DamageImmunityFrameDuration;
        }

        internal abstract void UpdateState();
        internal abstract void TakeDamage(float damageAmount);

        internal virtual void TakeHealth(float heathValue)
        {
            TakeHealthDamage(-heathValue);
        }

        protected void OnUnitDestroyed()
        {
            UnitDestroyed();
        }
        
        protected void OnDamageTaken()
        {
            DamageTaken();
        }

        protected void CooldownDamageImmunityFrame()
        {
            CurrentDamageImmunityTime -= Time.deltaTime;
        }
        
        protected void TakeHealthDamage(float damageAmount)
        {
            if (damageAmount >= CurrentHealth.Value)
            {
                CurrentHealth.Value = 0.0f;
                UnitDestroyed();
                return;
            }

            CurrentHealth.Value -= damageAmount;
        }
        
        protected void StartDamageImmunityWindow()
        {
            CurrentDamageImmunityTime = DamageImmunityFrameDuration;
        }
    }
}