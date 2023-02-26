using Gameplay.Background;
using Gameplay.Input;
using Gameplay.Movement;
using Gameplay.Services;
using Scriptables;
using UnityEngine;
using Zenject;

namespace Gameplay.Installers
{
    public sealed class GameplayServicesInstaller : MonoInstaller
    {
        [field: SerializeField] public GameBackgroundConfig GameBackgroundConfig { get; private set; }
        [field: SerializeField] public GameBackgroundView GameBackgroundView { get; private set; }

        [field: SerializeField] public PlayerInputConfig PlayerInputConfig { get; private set; }

        public override void InstallBindings()
        {
            InstallCurrentGameState();
            InstallBackground();
            InstallPlayerInput();
            InstallUnitMovement();
        }

        private void InstallCurrentGameState()
        {
            Container
                .Bind<CurrentGameState>()
                .AsSingle()
                .NonLazy();
        }

        private void InstallBackground()
        {
            Container
                .Bind<GameBackgroundConfig>()
                .FromInstance(GameBackgroundConfig)
                .AsSingle()
                .NonLazy();

            Container
                .Bind<GameBackgroundView>()
                .FromInstance(GameBackgroundView)
                .AsSingle()
                .NonLazy();

            Container
                .BindInterfacesAndSelfTo<GameBackground>()
                .AsSingle()
                .NonLazy();
        }

        private void InstallPlayerInput()
        {
            Container
                .Bind<PlayerInputConfig>()
                .FromInstance(PlayerInputConfig)
                .AsSingle()
                .NonLazy();
            
            Container
                .BindInterfacesAndSelfTo<PlayerInput>()
                .AsSingle()
                .NonLazy();
        }

        private void InstallUnitMovement()
        {
            Container
                .BindFactory<UnitMovementConfig, UnitMovementModel, UnitMovementModelFactory>()
                .AsSingle();
        }
    }
}