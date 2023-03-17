using System;
using Gameplay.Damage;
using Gameplay.Space.Star;
using UnityEngine;

namespace Gameplay.Space.Planets
{
    [RequireComponent(typeof(CircleCollider2D))]
    public sealed class PlanetView : MonoBehaviour, IDamagingView
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

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.TryGetComponent(out StarView starView))
            {
                CollisionEnter();
            }
        }
    }
}