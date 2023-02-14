using UnityEngine;

namespace Scriptables.Health
{
    [CreateAssetMenu(fileName = nameof(ShieldConfig), menuName = "Configs/Health/" + nameof(ShieldConfig))]
    public sealed class ShieldConfig : ScriptableObject
    {
        [field: SerializeField, Min(1f)] public float ShieldAmount { get; private set; } = 1.0f;
        [field: SerializeField, Min(0.1f)] public float Cooldown { get; private set; } = 0.1f;
    }
}