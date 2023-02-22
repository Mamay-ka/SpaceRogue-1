using System;
using UnityEngine;

namespace Gameplay.Movement
{
    [Serializable]
    public sealed class MovementConfig
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

        [Header("Dash")]
        [Min(0.1f)]
        [SerializeField] public float DashLength = 0.1f;
        [Min(0.1f)]
        [SerializeField] public float DashCooldown = 0.1f;
    }
}