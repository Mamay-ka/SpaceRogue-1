using Gameplay.Space.Planets;
using UnityEngine;

namespace Gameplay.Space.SpaceObjects.Scriptables
{
    [CreateAssetMenu(fileName = nameof(PlanetConfig), menuName = "Configs/Space/" + nameof(PlanetConfig))]
    public sealed class PlanetConfig : ScriptableObject
    {
        [field: SerializeField, Header("Prefab")] public PlanetView Prefab { get; private set; }
        
        [field: SerializeField, Min(0.1f), Header("Movement")] public float MinSpeed { get; private set; } = 0.1f;
        [field: SerializeField, Min(1f)] public float MaxSpeed { get; private set; } = 1f;
        [field: SerializeField, Range(0.01f, 1f)] public float RetrogradeMovementChance { get; private set; } = 0.1f;
        
        [field: SerializeField, Min(0.1f), Header("Size")] public float MinSize { get; private set; } = 0.1f;
        [field: SerializeField, Min(1f)] public float MaxSize { get; private set; } = 1f;

        [field: SerializeField] public float MinDamage { get; private set; }
        [field: SerializeField] public float MaxDamage { get; private set; }
    }
}