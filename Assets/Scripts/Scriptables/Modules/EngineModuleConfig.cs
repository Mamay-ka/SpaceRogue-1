using UnityEngine;

namespace Scriptables.Modules
{
    [CreateAssetMenu(fileName = nameof(EngineModuleConfig), menuName = "Configs/Modules/" + nameof(EngineModuleConfig))]
    public sealed class EngineModuleConfig : BaseModuleConfig
    {
        [Header("Speed")] 
        [Min(0.1f)]
        [SerializeField] public float maximumSpeed = 0.1f;
        [Min(0.1f)]
        [SerializeField] public float maximumBackwardSpeed = 0.1f;
        [Min(0.1f)]
        [SerializeField] public float accelerationTime = 0.1f;
        [Min(0.1f)]
        [SerializeField] public float stoppingSpeed = 0.3f;

        [Header("Turn speed")] 
        [Min(0.1f)]
        [SerializeField] public float startingTurnSpeed = 0.1f;
        [Min(0.1f)]
        [SerializeField] public float maximumTurnSpeed = 0.1f;
        [Min(0.1f)]
        [SerializeField] public float turnAccelerationTime = 0.1f;

        [Header("Leap speed")]//!!!!
        [Min(0.1f)]
        [SerializeField] public float leapSpeed = 0.1f;
        [Min(0.1f)]
        [SerializeField] public float leapTime = 0.1f;
    }
}