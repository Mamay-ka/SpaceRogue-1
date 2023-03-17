using UnityEngine;
using Zenject;
using Gameplay.Pooling;
using Gameplay.Space.SpaceObjects.Scriptables;
using Gameplay.Space.SpaceObjects;
using Gameplay.Space.Factories;
using Gameplay.Space.Planets;
using Gameplay.Space.SpaceObjects.SpaceObjectsEffects;

namespace Gameplay.Installers
{
    public sealed class SpaceObjectsInstaller : MonoInstaller
    {
        [field: SerializeField] public SpaceObjectsPool SpaceObjectsPool { get; private set; }
        //[field: SerializeField] public SpaceObjectSpawnConfig SpaceObjectSpawnConfig { get; private set; }

        public override void InstallBindings()
        {
            InstallSpaceObjectsPool();
            InstallPlanetFactories();
            InstallSpaceObjectFactories();
        }

        private void InstallSpaceObjectsPool()
        {
            Container
                .Bind<SpaceObjectsPool>()
                .FromInstance(SpaceObjectsPool)
                .AsSingle();
        }
        
        private void InstallPlanetFactories()
        {
            Container
                .BindFactory<Vector2, PlanetConfig, PlanetView, PlanetViewFactory>()
                .AsSingle();

            Container
                .BindFactory<PlanetView, PlanetMovement, PlanetMovementFactory>()
                .AsSingle();

            Container
                .BindFactory<Vector2, PlanetConfig, Planet, PlanetFactory>()
                .AsSingle();
        }

        private void InstallSpaceObjectFactories()
        {
            Container
                .BindFactory<Vector2, SpaceObjectConfig, SpaceObjectView, SpaceObjectViewFactory>()
                .AsSingle();
            
            Container
                .BindIFactory<SpaceObjectEffectConfig, SpaceObjectEffect>()
                .FromFactory<SpaceObjectEffectFactory>();
            
            Container
                .BindFactory<Vector2, SpaceObjectConfig, SpaceObject, SpaceObjectFactory>()
                .AsSingle();
        }
    }
}