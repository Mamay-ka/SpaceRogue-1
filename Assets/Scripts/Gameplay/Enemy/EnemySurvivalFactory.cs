using System;
using Gameplay.Survival;
using Zenject;

namespace Gameplay.Enemy
{
    public sealed class EnemySurvivalFactory : PlaceholderFactory<EntitySurvivalConfig, EntitySurvival>
    {
        private readonly EntitySurvivalFactory _entitySurvivalFactory;

        public event Action<EntitySurvival> EnemySurvivalCreated = _ => { };

        public EnemySurvivalFactory(EntitySurvivalFactory entitySurvivalFactory)
        {
            _entitySurvivalFactory = entitySurvivalFactory;
        }

        public override EntitySurvival Create(EntitySurvivalConfig enemySurvivalConfig)
        {
            var enemySurvival = _entitySurvivalFactory.Create(enemySurvivalConfig);
            EnemySurvivalCreated.Invoke(enemySurvival);
            return enemySurvival;
        }
    }
}