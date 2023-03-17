using System.Collections.Generic;
using Gameplay.Space.Planets;
using Gameplay.Space.SpaceObjects.Scriptables;
using Gameplay.Space.Star;
using Scriptables;
using UnityEngine;
using Utilities.Mathematics;
using Object = UnityEngine.Object;

namespace Gameplay.Space.Factories
{
    public sealed class LegacySpaceObjectFactory
    {
        private readonly StarSpawnConfig _starSpawnConfig;
        private readonly PlanetSpawnConfig _planetSpawnConfig;
        private readonly PlanetConfig _planetConfig;
        private readonly System.Random _random;
        public LegacySpaceObjectFactory(StarSpawnConfig starSpawnStarSpawnConfig, PlanetSpawnConfig planetSpawnConfig)
        {
            _starSpawnConfig = starSpawnStarSpawnConfig;
            _planetSpawnConfig = planetSpawnConfig;
            _random = new System.Random();
        }
        
        public (StarController, PlanetController[]) CreateStarSystem(Vector3 starSpawnPosition, Transform starsParent)
        {
            var config = PickStar(_starSpawnConfig.WeightConfigs, _random);
            float starSize = RandomPicker.PickRandomBetweenTwoValues(config.MinSize, config.MaxSize, _random);
            var starView = CreateStarView(config.Prefab, starSize, starSpawnPosition);
            
            int planetCount = RandomPicker.PickRandomBetweenTwoValues(config.MinPlanetCount, config.MaxPlanetCount, _random);
            var planets = new PlanetController[planetCount];
            
            if (planetCount <= 0) return (new StarController(starView, starsParent), planets);

            float[] planetOrbits = GetPlanetOrbitList(planetCount, config.MinOrbit, config.MaxOrbit, starSize, _random);

            for (int i = 0; i < planets.Length; i++)
            {
                var planetConfig = PickPlanet(_planetSpawnConfig.WeightConfigs, _random);
                float planetSize = RandomPicker.PickRandomBetweenTwoValues(planetConfig.MinSize, planetConfig.MaxSize, _random);
                float planetSpeed = RandomPicker.PickRandomBetweenTwoValues(planetConfig.MinSpeed, planetConfig.MaxSpeed, _random);
                float planetDamage = RandomPicker.PickRandomBetweenTwoValues(planetConfig.MinDamage, planetConfig.MaxDamage, _random);
                bool isPlanetMovingRetrograde = RandomPicker.TakeChance(planetConfig.RetrogradeMovementChance, _random);
                var planetView = CreatePlanetView(planetConfig.Prefab, planetSize, starSize, planetOrbits[i], starSpawnPosition);
                planets[i] = new PlanetController(planetView, starView, planetSpeed, isPlanetMovingRetrograde, planetDamage);
            }
            return (new StarController(starView, starsParent), planets);
        }

        private static StarView CreateStarView(StarView prefab, float size, Vector3 spawnPosition)
        {
            var viewGo = Object.Instantiate(prefab, spawnPosition, Quaternion.identity);
            viewGo.transform.localScale = new Vector3(size, size);
            return viewGo;
        }
        
        private static PlanetView CreatePlanetView(PlanetView prefab, float size, float starSize, float orbit, Vector3 starPosition)
        {
            var planetSpawnPosition = starPosition + new Vector3(0, starSize + orbit + size / 2, 0);
            var viewGo = Object.Instantiate(prefab, planetSpawnPosition, Quaternion.identity);
            viewGo.transform.localScale = new Vector3(size, size);
            return viewGo;
        }

        private static float[] GetPlanetOrbitList(int planetCount, float minOrbit, float maxOrbit, float starSize, System.Random random)
        {
            var orbits = new float[planetCount];
            float realMinOrbit = starSize / 2 + minOrbit;
            float realMaxOrbit = starSize / 2 + maxOrbit;

            if (planetCount == 1)
            {
                orbits[0] = RandomPicker.PickRandomBetweenTwoValues(realMinOrbit, realMaxOrbit, random);
                return orbits;
            }

            float orbitChunk = realMaxOrbit - realMinOrbit / planetCount;
            
            for (int i = 0; i < planetCount; i++)
            {
                float minChunkOrbit = realMinOrbit + i * orbitChunk;
                float maxChunkOrbit = realMinOrbit + (i + 1) * orbitChunk;
                orbits[i] = RandomPicker.PickRandomBetweenTwoValues(minChunkOrbit, maxChunkOrbit, random);
            }

            return orbits;
        }

        private static StarConfig PickStar(List<WeightConfig<StarConfig>> weightConfigs, System.Random random) 
            => RandomPicker.PickOneElementByWeights(weightConfigs, random);

        private static PlanetConfig PickPlanet(List<WeightConfig<PlanetConfig>> weightConfigs, System.Random random) => 
            RandomPicker.PickOneElementByWeights(weightConfigs, random);

    }
}