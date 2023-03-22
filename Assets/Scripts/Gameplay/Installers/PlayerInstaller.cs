using Gameplay.Movement;
using Gameplay.Player;
using Gameplay.Player.Movement;
using Gameplay.Player.Weapon;
using Gameplay.Shooting;
using Gameplay.Shooting.Scriptables;
using Gameplay.Survival;
using Scriptables;
using UnityEngine;
using Zenject;

namespace Gameplay.Installers
{
    public sealed class PlayerInstaller : MonoInstaller
    {
        [field: SerializeField] public PlayerView PlayerViewPrefab { get; private set; }
        [field: SerializeField] public PlayerConfig PlayerConfig { get; private set; }
        
        public override void InstallBindings()
        {
            InstallPlayerView();
            InstallPlayerMovement();
            InstallPlayerHealth();
            InstallPlayerWeapon();
            InstallPlayer();
        }

        private void InstallPlayerView()
        {
            Container
                .Bind<PlayerView>()
                .FromInstance(PlayerViewPrefab)
                .WhenInjectedInto<PlayerViewFactory>();

            Container
                .BindFactory<Vector2, PlayerView, PlayerViewFactory>()
                .AsSingle();
        }

        private void InstallPlayerMovement()
        {
            Container
                .Bind<UnitMovementConfig>()
                .FromInstance(PlayerConfig.UnitMovement)
                .WhenInjectedInto<PlayerMovement>();
            
            Container
                .Bind<UnitMovementConfig>()
                .FromInstance(PlayerConfig.UnitMovement)
                .WhenInjectedInto<PlayerTurning>();

            Container//!!!!!!!!!!
                .Bind<UnitMovementConfig>()
                .FromInstance(PlayerConfig.UnitMovement)
                .WhenInjectedInto<PlayerDash>();

            Container//!!!!
                .BindFactory<PlayerView, PlayerDash, PlayerDashFactory>()
                .AsSingle();

            Container
                .BindFactory<PlayerView, PlayerMovement, PlayerMovementFactory>()
                .AsSingle();
            
            Container
                .BindFactory<PlayerView, PlayerTurning, PlayerTurningFactory>()
                .AsSingle();

            Container//!!!
                .Bind<PlayerDash>()
                .AsCached();

            Container
                .Bind<PlayerMovement>()
                .AsCached();

            Container
                .Bind<PlayerTurning>()
                .AsCached();
        }

        private void InstallPlayerHealth()
        {
            Container.Bind<EntitySurvivalConfig>()
                .FromInstance(PlayerConfig.Survival)
                .WhenInjectedInto<PlayerSurvivalFactory>();
            
            Container
                .BindFactory<EntitySurvival, PlayerSurvivalFactory>()
                .AsSingle();
        }

        private void InstallPlayerWeapon()
        {
            Container
                .Bind<MountedWeaponConfig>()
                .FromInstance(PlayerConfig.StartingWeapon)
                .WhenInjectedInto<PlayerWeaponFactory>();

            Container
                .BindFactory<PlayerView, PlayerWeapon, PlayerWeaponFactory>()
                .AsSingle();

            Container
                .Bind<PlayerWeapon>()
                .AsCached();
        }

        private void InstallPlayer()
        {
            Container
                .BindFactory<Vector2, Player.Player, PlayerFactory>()
                .AsSingle();
        }
    }
}