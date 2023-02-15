using Scriptables.Health;

namespace Gameplay.Health
{
    public class HealthOnlyModel : BaseHealthModel
    {
        public HealthOnlyModel(IHealthInfo healthInfo) : base(healthInfo)
        {
        }
        
        internal override void TakeDamage(float damageAmount)
        {
            if (CurrentDamageImmunityTime > 0.0f) return;

            StartDamageImmunityWindow();
            OnDamageTaken();

            TakeHealthDamage(damageAmount);
        }

        internal override void UpdateState()
        {
            if (CurrentDamageImmunityTime >= 0.0f) CooldownDamageImmunityFrame();

            if (CurrentHealth.Value < MaximumHealth.Value)
            {
                CurrentHealth.Value += RegenAmountPerDeltaTime;
            }
        }
    }
}