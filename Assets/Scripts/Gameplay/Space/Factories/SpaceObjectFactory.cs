using System.Collections.Generic;
using Gameplay.Space.SpaceObjects;
using Gameplay.Space.SpaceObjects.Scriptables;
using Gameplay.Space.SpaceObjects.SpaceObjectsEffects;
using UnityEngine;
using Zenject;

namespace Gameplay.Space.Factories
{
    public class SpaceObjectFactory : PlaceholderFactory<Vector2, SpaceObjectConfig, SpaceObject>
    {
        private readonly SpaceObjectViewFactory _viewFactory;
        private readonly IFactory<SpaceObjectEffectConfig, SpaceObjectEffect> _spaceObjectEffectFactory;

        public SpaceObjectFactory(SpaceObjectViewFactory viewFactory, IFactory<SpaceObjectEffectConfig, SpaceObjectEffect> spaceObjectEffectFactory)
        {
            _viewFactory = viewFactory;
            _spaceObjectEffectFactory = spaceObjectEffectFactory;
        }
        
        public override SpaceObject Create(Vector2 position, SpaceObjectConfig config)
        {
            var view = _viewFactory.Create(position, config);
            List<SpaceObjectEffect> effects = new();
            foreach (var effectConfig in config.Effects)
            {
                effects.Add(_spaceObjectEffectFactory.Create(effectConfig));
            }
            return new SpaceObject(view, effects);
        }
    }
}