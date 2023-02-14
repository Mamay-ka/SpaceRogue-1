using Abstracts;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Space.Obstacle
{
    public sealed class ObstacleController : BaseController
    {
        private readonly ObstacleView _obstacleView;
        private readonly float _obstacleForce;

        private readonly Dictionary<UnitView, ObstacleContactInfo> _unitCollection = new();

        public ObstacleController(ObstacleView obstacleView, float obstacleForce)
        {
            _obstacleView = obstacleView;
            _obstacleForce = obstacleForce;

            _obstacleView.OnTriggerEnter += OnObstacleEnter;
            _obstacleView.OnTriggerStay += OnObstacleStay;
            _obstacleView.OnTriggerExit += OnObstacleExit;

            AddGameObject(obstacleView.gameObject);
        }

        protected override void OnDispose()
        {
            _obstacleView.OnTriggerEnter -= OnObstacleEnter;
            _obstacleView.OnTriggerStay -= OnObstacleStay;
            _obstacleView.OnTriggerExit -= OnObstacleExit;
        }

        private void OnObstacleEnter(UnitView unitView)
        {
            if (_unitCollection.ContainsKey(unitView))
            {
                return;
            }

            var rigidbody = unitView.GetComponent<Rigidbody2D>();
            var currentDirection = unitView.transform.TransformDirection(Vector3.up).normalized;
            var dot = Vector2.Dot(currentDirection, rigidbody.velocity.normalized);
            
            var info = new ObstacleContactInfo() { StartDirection = currentDirection, IsVelocitySameDirection = dot >= 0 };
            
            _unitCollection.Add(unitView, info);
        }
        
        private void OnObstacleStay(UnitView unitView)
        {
            if (_unitCollection.TryGetValue(unitView, out var obstacleContactInfo))
            {
                var rigidbody = unitView.GetComponent<Rigidbody2D>();
                var direction = obstacleContactInfo.IsVelocitySameDirection ? -obstacleContactInfo.StartDirection : obstacleContactInfo.StartDirection;
                rigidbody.AddForce(direction * _obstacleForce, ForceMode2D.Impulse);
            }
        }
        
        private void OnObstacleExit(UnitView unitView)
        {
            if (_unitCollection.ContainsKey(unitView))
            {
                _unitCollection.Remove(unitView);
            }
        }
    }
}