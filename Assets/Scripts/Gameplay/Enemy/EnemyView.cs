using Abstracts;
using Gameplay.Damage;
using Gameplay.Space.Star;
using UnityEngine;

namespace Gameplay.Enemy
{
    public sealed class EnemyView : UnitView
    {
        public new void OnTriggerEnter2D(Collider2D collider)
        {
            CollisionEnter(collider.gameObject);
        }

        public new void OnCollisionEnter2D(Collision2D collision)
        {
            CollisionEnter(collision.gameObject);
        }

        private void CollisionEnter(GameObject gameObject)
        {
            if (gameObject.TryGetComponent(out IDamagingView view) && !gameObject.TryGetComponent(out StarView _))
            {
                TakeDamage(view);
            }
        }
    }
}