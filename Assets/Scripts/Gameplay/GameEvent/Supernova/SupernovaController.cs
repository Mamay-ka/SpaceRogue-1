using Abstracts;
using Gameplay.Mechanics.Timer;
using Gameplay.Space.Star;
using Scriptables.GameEvent;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Reactive.SubscriptionProperty;

namespace Gameplay.GameEvent
{
    public sealed class SupernovaController : BaseController
    {
        private const int CollapseSpeed = 25;
        private const float ScaleCoeff = 1.1f;
        private const float ShockwaveRadiusCoeff = 0.9f;

        private readonly SupernovaGameEventConfig _supernovaGameEventConfig;
        private readonly Timer _explosionTimer;
        private readonly StarView _starView;
        private readonly CircleCollider2D _starCircleCollider;
        private readonly SpriteRenderer _spriteRenderer;
        private readonly Color _starViewColor;
        private readonly Vector3 _starViewScale;
        
        private Color _currentColor;
        private Vector3 _currentScale;
        private bool _isSupernova;

        public SubscribedProperty<bool> OnDestroy = new();

        public SupernovaController(SupernovaGameEventConfig config, StarView starView)
        {
            _supernovaGameEventConfig = config;
            _starView = starView;
            _starCircleCollider = _starView.GetComponent<CircleCollider2D>();
            _spriteRenderer = _starView.GetComponent<SpriteRenderer>();
            _starViewColor = _spriteRenderer.color;
            _starViewScale = _starView.transform.localScale;

            _explosionTimer = new(_supernovaGameEventConfig.TimeToExplosionInSeconds);
            _explosionTimer.Start();

            EntryPoint.SubscribeToUpdate(PrepareSupernova);
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            _explosionTimer.Dispose();
        }

        private void PrepareSupernova()
        {
            if (_starView == null)
            {
                EntryPoint.UnsubscribeFromUpdate(PrepareSupernova);
                OnDestroy.Value = true;
                return;
            }

            if (_explosionTimer.InProgress)
            {
                ChangeColorRepeat(_spriteRenderer, ref _currentColor, _supernovaGameEventConfig.WarningColor, _starViewColor);

                if (_explosionTimer.CurrentValue < 1f)
                {
                    ChangeScale(_starView.transform, ref _currentScale, _starView.transform.localScale, _starViewScale * ScaleCoeff);
                }
            }

            if (_explosionTimer.IsExpired)
            {
                EntryPoint.UnsubscribeFromUpdate(PrepareSupernova);
                EntryPoint.SubscribeToUpdate(StartSupernova);
            }
        }

        private void StartSupernova()
        {
            if(_starView == null)
            {
                EntryPoint.UnsubscribeFromUpdate(StartSupernova);
                OnDestroy.Value = true;
                return;
            }

            if (_starView.transform.localScale.x <= 0)
            {
                _spriteRenderer.sprite = _supernovaGameEventConfig.SupernovaSprite;
                _spriteRenderer.color = _supernovaGameEventConfig.ShockwaveColor;
                _currentColor = _spriteRenderer.color;
                _starView.transform.localScale = Vector3.zero;
                _starView.Init(new(_supernovaGameEventConfig.ShockwaveDamage));
                _starCircleCollider.isTrigger = true;
                _starCircleCollider.offset = Vector2.zero;

                _isSupernova = true;
            }

            if (_isSupernova)
            {
                if (_starView.transform.localScale.x <= _supernovaGameEventConfig.ShockwaveRadius)
                {
                    if (_starView.transform.localScale.x >= _supernovaGameEventConfig.ShockwaveRadius * ShockwaveRadiusCoeff)
                    {
                        ChangeColor(_spriteRenderer, ref _currentColor, _spriteRenderer.color, Color.clear,
                                _supernovaGameEventConfig.ShockwaveSpeed * Time.deltaTime);
                        ChangeColor(_starView.MinimapIconSpriteRenderer, 
                            ref _currentColor, _starView.MinimapIconSpriteRenderer.color, Color.clear,
                            _supernovaGameEventConfig.ShockwaveSpeed * Time.deltaTime);
                    }
                    ChangeScale(_starView.transform, ref _currentScale, _starView.transform.localScale,
                                Vector3.one * (_supernovaGameEventConfig.ShockwaveRadius + 1),
                                _supernovaGameEventConfig.ShockwaveSpeed);
                    PushAllRigitbodies(_starCircleCollider, _starView.transform.position, _supernovaGameEventConfig.ShockwaveForce);
                    return;
                }

                _starView.transform.localScale = Vector3.one * _supernovaGameEventConfig.ShockwaveRadius;
                _spriteRenderer.color = Color.clear;
                _starCircleCollider.enabled = false;
                _starView.MinimapIconSpriteRenderer.enabled = false;

                EntryPoint.UnsubscribeFromUpdate(StartSupernova);
                OnDestroy.Value = true;
                return;
            }

            ChangeScale(_starView.transform, ref _currentScale, _starView.transform.localScale, -Vector3.one, CollapseSpeed);
        }

        private void ChangeColorRepeat(SpriteRenderer spriteRenderer, ref Color currentColor, Color a, Color b, float timeCoef = 1)
        {
            ChangeColor(spriteRenderer, ref currentColor, a, b, Mathf.PingPong(Time.time * timeCoef, 1));
        }

        private void ChangeColor(SpriteRenderer spriteRenderer, ref Color currentColor, Color a, Color b, float t)
        {
            currentColor = Color.Lerp(a, b, t);
            spriteRenderer.color = currentColor;
        }

        private void ChangeScale(Transform transform, ref Vector3 currentScale, Vector3 a, Vector3 b, float speed = 1)
        {
            currentScale = Vector3.Lerp(a, b, speed * Time.deltaTime);
            transform.localScale = currentScale;
        }

        private void PushAllRigitbodies(Collider2D collider, Vector3 position, float shockwaveForce)
        {
            var colliders = new List<Collider2D>();
            Physics2D.OverlapCollider(collider, new ContactFilter2D().NoFilter(), colliders);

            foreach (var item in colliders)
            {
                if (item.TryGetComponent(out Rigidbody2D rigidbody))
                {
                    var direction = (item.transform.position - position).normalized;
                    rigidbody.AddForce(shockwaveForce * direction, ForceMode2D.Impulse);
                }
            }
        }
    }
}