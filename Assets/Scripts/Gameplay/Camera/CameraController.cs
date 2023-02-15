using Abstracts;
using Gameplay.Player;
using UnityEngine;

namespace Gameplay.Camera
{
    public sealed class CameraController : BaseController
    {
        private readonly CameraView _cameraView;
        private readonly PlayerController _playerController;
        private const int CameraZAxisOffset = -10;
        
        private Transform _cameraTransform;
        private Transform _playerTransform;

        public CameraController(PlayerController playerController)
        {
            _cameraView = UnityEngine.Camera.main!.GetComponent<CameraView>();
            _playerController = playerController;
            _cameraTransform = _cameraView.gameObject.transform;
            _playerTransform = _playerController.View.gameObject.transform;
            EntryPoint.SubscribeToUpdate(FollowPlayer);
            _playerController.PlayerDestroyed += OnPlayerDestroyed;
        }
        
        public void OnPlayerDestroyed()
        {
            EntryPoint.UnsubscribeFromUpdate(FollowPlayer);
        }

        private void FollowPlayer()
        {
            if(_playerTransform == null)
            {
                return;
            }

            var position = _playerTransform.position;
            _cameraTransform.position = new(position.x, position.y, position.z + CameraZAxisOffset);
        }
    }
}