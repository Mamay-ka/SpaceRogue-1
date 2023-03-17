using System;
using Gameplay.Damage;
using Gameplay.Space.Planets;
using Gameplay.Space.Star;
using UnityEngine;

namespace Gameplay.GameEvent.Comet
{
    [RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D), typeof(TrailRenderer))]
    public sealed class CometView : MonoBehaviour, IDamagingView
    {
        public event Action CollisionEnter = () => { };
        public DamageModel DamageModel { get; private set; }

        public void Init(DamageModel damageModel)
        {
            DamageModel = damageModel;
        }

        public void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.TryGetComponent(out StarView starView) || collision.gameObject.TryGetComponent(out PlanetView planetView))
            {
                CollisionEnter();
            }
        }
    }
}