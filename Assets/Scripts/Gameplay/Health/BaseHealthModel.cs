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

        internal BaseHealthModel(IHealthInfo healthInfo)
        {
            MaximumHealth = new SubscribedProperty<float>(healthInfo.MaximumHealth);
            CurrentHealth = new SubscribedProperty<float>(healthInfo.StartingHealth);
            HealthRegenAmount = healthInfo.HealthRegen;
            DamageImmunityFrameDuration = healthInfo.DamageImmunityFrameDuration;
        }
        
        internal abstract void UpdateState();
        internal abstract void TakeDamage(float damageAmount);

        internal virtual void TakeHealth(float heathValue)
        {
            TakeHealthDamage(-heathValue);
        }

        protected void OnUnitDestroyed()
        {
            UnitDestroyed.Invoke();
        }
        
        protected void OnDamageTaken()
        {
            DamageTaken.Invoke();
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
                UnitDestroyed.Invoke();
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