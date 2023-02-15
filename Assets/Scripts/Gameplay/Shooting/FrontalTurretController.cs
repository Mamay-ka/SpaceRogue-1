using Abstracts;
using Gameplay.Mechanics.Timer;
using Scriptables.Modules;
using UnityEngine;
using Utilities.ResourceManagement;

namespace Gameplay.Shooting
{
    public abstract class FrontalTurretController : BaseController
    {
        public bool IsOnCooldown => CooldownTimer.InProgress;
        
        protected Timer CooldownTimer;

        protected readonly TurretModuleConfig Config;
        protected readonly ProjectileFactory ProjectileFactory;

        private readonly ResourcePath _gunPointPrefab = new(Constants.Prefabs.Stuff.GunPoint);

        public FrontalTurretController(TurretModuleConfig config, Transform gunPointParentTransform, UnitType unitType)
        {
            Config = config;
            var gunPointView = ResourceLoader.LoadPrefab(_gunPointPrefab);
            
            var turretPoint = Object.Instantiate(
                gunPointView,
                gunPointParentTransform.position + gunPointParentTransform.TransformDirection(
                    0.6f * gunPointParentTransform.localScale.y * Vector3.up),
                gunPointParentTransform.rotation
            );
            turretPoint.transform.parent = gunPointParentTransform;
            
            ProjectileFactory = new ProjectileFactory(Config.ProjectileConfig, Config.ProjectileConfig.Prefab, 
                turretPoint.transform, unitType);

            CooldownTimer = new Timer(config.SpecificWeapon.Cooldown);
            
            AddGameObject(turretPoint);
        }

        protected override void OnDispose()
        {
            CooldownTimer.Dispose();
        }

        public abstract void CommenceFiring();
    }
}