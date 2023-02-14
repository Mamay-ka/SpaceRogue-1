using Gameplay.Damage;
using UnityEngine;

namespace Gameplay.Space.Star
{
    [RequireComponent(typeof(CircleCollider2D), typeof(SpriteRenderer))]
    public sealed class StarView : MonoBehaviour, IDamagingView
    {
        public DamageModel DamageModel { get; private set; }
        public bool InGameEvent { get; set; }

        public void Init(DamageModel damageModel)
        {
            DamageModel = damageModel;
        }
    }
}