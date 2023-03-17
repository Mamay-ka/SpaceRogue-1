using System;
using Object = UnityEngine.Object;

namespace Gameplay.Space.Planets
{
    public class Planet : IDisposable
    {
        private readonly PlanetView _planetView;
        private readonly PlanetMovement _planetMovement;

        public Planet(PlanetView planetView, PlanetMovement planetMovement)
        {
            _planetView = planetView;
            _planetMovement = planetMovement;

            _planetView.CollisionEnter += Dispose;
        }

        public void Dispose()
        {
            _planetView.CollisionEnter -= Dispose;
            
            _planetMovement.Dispose();
            
            Object.Destroy(_planetView.gameObject);
        }
    }
}