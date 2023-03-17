using System.Collections.Generic;
using Scriptables;
using UnityEngine;

namespace Gameplay.Space.SpaceObjects.Scriptables
{
    [CreateAssetMenu(fileName = nameof(SpaceObjectSpawnConfig), menuName = "Configs/Space/" + nameof(SpaceObjectSpawnConfig))]
    public sealed class SpaceObjectSpawnConfig : ScriptableObject
    {
        [field: SerializeField] public List<WeightConfig<SpaceObjectConfig>> SpaceObjectWeights { get; private set; }
    }
}