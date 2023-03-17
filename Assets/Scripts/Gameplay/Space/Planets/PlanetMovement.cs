using System;
using Services;

namespace Gameplay.Space.Planets
{
    public class PlanetMovement : IDisposable
    {
        private readonly PlanetView _view;
        private readonly Updater _updater;

        public PlanetMovement(PlanetView view, Updater updater)
        {
            _view = view;
            _updater = updater;
            
            _updater.SubscribeToUpdate(Move);
        }

        public void Dispose()
        {
            _updater.UnsubscribeFromUpdate(Move);
        }

        private void Move(float deltaTime)
        {
            //TODO write physical movement
            /*if (_starView is not null)
            {
                _view.transform.RotateAround(
                    _starView.transform.position,
                    _isMovingRetrograde ? Vector3.forward : Vector3.back,
                    _currentSpeed * deltaTime
                );
            }*/
        }
    }
}