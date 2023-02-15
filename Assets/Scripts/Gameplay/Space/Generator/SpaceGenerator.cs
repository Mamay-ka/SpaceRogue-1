using Scriptables.Enemy;
using Scriptables.Space;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utilities.Mathematics;
using Random = System.Random;

namespace Gameplay.Space.Generator
{
    public abstract class SpaceGenerator
    {
        private const int DefaultMooreCount = 4;
        private const int DefaultVonNeumannCount = 2;
        private const float NoiseScale = 0.1f;

        protected readonly TileBase _borderTileBase;
        protected readonly TileBase _borderMaskTileBase;
        protected readonly TileBase _nebulaTileBase;
        protected readonly TileBase _nebulaMaskTileBase;

        protected readonly int _widthMap;
        protected readonly int _heightMap;
        protected readonly int _outerBorder;
        protected readonly int _innerBorder;
        protected readonly SmoothMapType _smoothMapType;
        protected readonly int _factorSmooth;
        protected readonly RandomType _randomType;
        protected readonly float _chance;

        protected readonly int _starCount;
        protected readonly int _starRadius;

        private readonly int _enemyCount;
        private readonly int _enemyRadius;

        protected readonly int[,] _borderMap;
        protected readonly int[,] _nebulaMap;
        protected readonly int[,] _spaceObjectsMap;

        private List<Point> _availablePoints = new();

        public SpaceGenerator(SpaceView spaceView,
                              SpaceConfig spaceConfig,
                              StarSpawnConfig starSpawnConfig,
                              EnemySpawnConfig enemySpawnConfig)
        {
            _borderTileBase = spaceConfig.BorderTileBase;
            _borderMaskTileBase = spaceConfig.BorderMaskTileBase;
            _nebulaTileBase = spaceConfig.NebulaTileBase;
            _nebulaMaskTileBase = spaceConfig.NebulaMaskTileBase;

            _widthMap = spaceConfig.WidthMap;
            _heightMap = spaceConfig.HeightMap;
            _outerBorder = spaceConfig.OuterBorder;
            _innerBorder = spaceConfig.InnerBorder;
            _smoothMapType = spaceConfig.SmoothMapType;
            _factorSmooth = spaceConfig.FactorSmooth;
            _randomType = spaceConfig.RandomType;
            _chance = spaceConfig.Chance;

            _starCount = spaceConfig.StarCount;

            if (spaceConfig.AutoRadius)
            {
                _starRadius = GetStarRadius(starSpawnConfig, spaceView.NebulaTilemap);
            }
            else
            {
                _starRadius = spaceConfig.ManualRadius;
            }

            _enemyCount = enemySpawnConfig.EnemyGroupsSpawnPoints.Count;
            _enemyRadius = GetEnemyRadius(enemySpawnConfig, spaceView.NebulaMaskTilemap);

            _borderMap = new int[_widthMap + 2 * _outerBorder, _heightMap + 2 * _outerBorder];
            _nebulaMap = new int[_widthMap, _heightMap];
            _spaceObjectsMap = new int[_widthMap, _heightMap];
        }

        public void Generate()
        {
            FillBorder(_borderMap, _widthMap, _heightMap, _outerBorder);
            FillNebula(_nebulaMap, _innerBorder, NoiseScale, _randomType, _smoothMapType);
            StarSpawnPoints(_spaceObjectsMap, _nebulaMap, _starCount, _starRadius);
            PlayerSpawnPoint(_spaceObjectsMap);
            EnemiesSpawnPoints(_spaceObjectsMap, _enemyCount, _enemyRadius);
            Draw();
        }

        protected abstract void Draw();

        private int GetStarRadius(StarSpawnConfig starSpawnConfig, Tilemap tilemap)
        {
            var maxStarSize = default(float);
            var maxOrbit = default(float);

            foreach (var item in starSpawnConfig.WeightConfigs)
            {
                maxStarSize = Mathf.Max(maxStarSize, item.Config.MaxSize);
                maxOrbit = Mathf.Max(maxOrbit, item.Config.MaxOrbit);
            }

            var radius = (maxStarSize / 2 + maxOrbit)
                / Mathf.Max(tilemap.cellSize.x * tilemap.transform.localScale.x,
                            tilemap.cellSize.y * tilemap.transform.localScale.y);
            Debug.Log($"Radius: {Mathf.CeilToInt(radius)}");

            return Mathf.CeilToInt(radius);
        }

        private int GetEnemyRadius(EnemySpawnConfig enemySpawnConfig, Tilemap tilemap)
        {
            var maxCount = default(int);

            foreach (var item in enemySpawnConfig.EnemyGroupsSpawnPoints)
            {
                maxCount = Mathf.Max(maxCount, item.GroupCount);
            }

            var radius = maxCount
                / Mathf.Max(tilemap.cellSize.x * tilemap.transform.localScale.x,
                            tilemap.cellSize.y * tilemap.transform.localScale.y);
            Debug.Log($"EnemyRadius: {Mathf.CeilToInt(radius)}");

            return Mathf.CeilToInt(radius);
        }

        private void FillBorder(int[,] map, int widthMap, int heightMap, int outerBorder)
        {
            if (map == null)
            {
                return;
            }

            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    if (x <= outerBorder - 1
                        || x >= widthMap + outerBorder
                        || y <= outerBorder - 1
                        || y >= heightMap + outerBorder)
                    {
                        map[x, y] = (int)CellType.Border;
                    }
                }
            }
        }

        private void FillNebula(int[,] map, int innerBorder, float noiseScale, RandomType randomType, SmoothMapType smoothMapType)
        {
            if (map == null)
            {
                return;
            }

            RandomFillLevel(randomType, map, innerBorder, noiseScale);
            SmoothMap(smoothMapType, map, 1, false);
        }

        #region RandomFill
        private void RandomFillLevel(RandomType randomType, int[,] map, int innerBorder, float noiseScale)
        {
            var pseudoRandom = new Random();

            switch (randomType)
            {
                case RandomType.Random:
                    StandardRandomMapFill(map, innerBorder, pseudoRandom);
                    break;
                case RandomType.PerlinNoise:
                    PerlinNoiseMapFill(map, innerBorder, pseudoRandom, noiseScale);
                    break;
            }
        }

        private void StandardRandomMapFill(int[,] map, int innerBorder, Random pseudoRandom)
        {
            for (int x = innerBorder; x < map.GetLength(0) - innerBorder; x++)
            {
                for (int y = innerBorder; y < map.GetLength(1) - innerBorder; y++)
                {
                    map[x, y] = RandomPicker.TakeChance(_chance, pseudoRandom) ? (int)CellType.Obstacle : (int)CellType.None;
                }
            }
        }

        private void PerlinNoiseMapFill(int[,] map, int innerBorder, Random pseudoRandom, float noiseScale)
        {
            var xOffset = pseudoRandom.Next(-map.GetLength(0) / 2, map.GetLength(0) / 2);
            var yOffset = pseudoRandom.Next(-map.GetLength(1) / 2, map.GetLength(1) / 2);

            for (int x = innerBorder; x < map.GetLength(0) - innerBorder; x++)
            {
                for (int y = innerBorder; y < map.GetLength(1) - innerBorder; y++)
                {
                    var noise = Mathf.PerlinNoise(x * noiseScale + xOffset, y * noiseScale + yOffset);
                    _nebulaMap[x, y] = noise >= 0.99f - _chance ? (int)CellType.Obstacle : (int)CellType.None;
                }
            }
        }
        #endregion

        #region SmoothMap
        private void SmoothMap(SmoothMapType smoothMapType, int[,] map, int radius, bool edgesAreWalls)
        {
            for (int i = 0; i < _factorSmooth; i++)
            {
                for (int x = 0; x < _widthMap; x++)
                {
                    for (int y = 0; y < _heightMap; y++)
                    {
                        var neighbourCount = GetSurroundingNeighbourCount(smoothMapType, map, x, y, radius, edgesAreWalls, out int defaultCount);

                        if (neighbourCount > defaultCount)
                        {
                            _nebulaMap[x, y] = (int)CellType.Obstacle;
                        }
                        else if (neighbourCount < defaultCount)
                        {
                            _nebulaMap[x, y] = (int)CellType.None;
                        }
                    }
                }
            }
        }

        private int GetSurroundingNeighbourCount(SmoothMapType smoothMapType, int[,] map, int gridX, int gridY, int radius, bool edgesAreWalls, out int defaultCount)
        {
            var count = default(int);
            defaultCount = 0;

            switch (smoothMapType)
            {
                case SmoothMapType.MooreNeighborhood:
                    count = MooreNeighborhoodCount(map, gridX, gridY, radius, edgesAreWalls);
                    defaultCount = DefaultMooreCount;
                    break;
                case SmoothMapType.VonNeumannNeighborhood:
                    count = VonNeumannNeighborhoodCount(map, gridX, gridY, edgesAreWalls);
                    defaultCount = DefaultVonNeumannCount;
                    break;
            }

            return count;
        }

        private int MooreNeighborhoodCount(int[,] map, int gridX, int gridY, int radius, bool edgesAreWalls)
        {
            var neighbourCount = 0;

            for (int neighbourX = gridX - radius; neighbourX <= gridX + radius; neighbourX++)
            {
                for (int neighbourY = gridY - radius; neighbourY <= gridY + radius; neighbourY++)
                {
                    if (neighbourX >= 0 && neighbourX < _widthMap && neighbourY >= 0 && neighbourY < _heightMap)
                    {
                        if (neighbourX != gridX || neighbourY != gridY)
                        {
                            neighbourCount += map[neighbourX, neighbourY] > 0 ? 1 : 0;
                        }
                    }
                    else if (edgesAreWalls)
                    {
                        neighbourCount++;
                    }
                }
            }

            return neighbourCount;
        }

        private int VonNeumannNeighborhoodCount(int[,] map, int gridX, int gridY, bool edgesAreWalls)
        {
            var neighbourCount = 0;

            if (edgesAreWalls && (gridX - 1 == 0 || gridX + 1 == map.GetLength(0) || gridY - 1 == 0 || gridY + 1 == map.GetLength(1)))
            {
                neighbourCount++;
            }

            if (gridX - 1 > 0)
            {
                neighbourCount += map[gridX - 1, gridY] > 0 ? 1 : 0;
            }

            if (gridY - 1 > 0)
            {
                neighbourCount += map[gridX, gridY - 1] > 0 ? 1 : 0;
            }

            if (gridX + 1 < map.GetLength(0))
            {
                neighbourCount += map[gridX + 1, gridY] > 0 ? 1 : 0;
            }

            if (gridY + 1 < map.GetLength(1))
            {
                neighbourCount += map[gridX, gridY + 1] > 0 ? 1 : 0;
            }

            return neighbourCount;
        } 
        #endregion

        #region SpaceObjectSpawn
        private void StarSpawnPoints(int[,] spaceObjectsMap, int[,] map, int starCount, int radius)
        {
            if (spaceObjectsMap == null)
            {
                return;
            }

            if (map == null)
            {
                return;
            }

            var pseudoRandom = new Random();
            _availablePoints = CheckAvailablePoints(map, radius);
            TrySetCellOnMap(spaceObjectsMap, starCount, radius, pseudoRandom, CellType.Star);
        }

        private void PlayerSpawnPoint(int[,] spaceObjectsMap)
        {
            var pseudoRandom = new Random();
            TrySetCellOnMap(spaceObjectsMap, 1, 0, pseudoRandom, CellType.Player);
        }

        private void EnemiesSpawnPoints(int[,] spaceObjectsMap, int enemiesCount, int radius)
        {
            var pseudoRandom = new Random();
            TrySetCellOnMap(spaceObjectsMap, enemiesCount, radius, pseudoRandom, CellType.Enemy);
        }

        private void TrySetCellOnMap(int[,] map, int cellCount, int radius, Random pseudoRandom, CellType cellType)
        {
            if(_availablePoints == null)
            {
                return;
            }

            var count = 0;

            while (count < cellCount)
            {
                if (_availablePoints.Count == 0)
                {
                    Debug.LogWarning($"Not enough space for all \"{cellType}\" | Count = {count}");
                    break;
                }

                var i = pseudoRandom.Next(_availablePoints.Count);
                map[_availablePoints[i].X, _availablePoints[i].Y] = (int)cellType;
                RemovePoints(ref _availablePoints, _availablePoints[i].X, _availablePoints[i].Y, radius);
                count++;
            }
        }

        private List<Point> CheckAvailablePoints(int[,] map, int radius)
        {
            var points = new List<Point>();

            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    if (map[x, y] == (int)CellType.None)
                    {
                        if (MooreNeighborhoodCount(map, x, y, radius, true) == 0)
                        {
                            points.Add(new Point(x, y));
                        }
                    }
                }
            }

            return points;
        }

        private void RemovePoints(ref List<Point> points, int gridX, int gridY, int radius)
        {
            for (int neighbourX = gridX - radius; neighbourX <= gridX + radius; neighbourX++)
            {
                for (int neighbourY = gridY - radius; neighbourY <= gridY + radius; neighbourY++)
                {
                    var item = new Point(neighbourX, neighbourY);

                    if (points.Contains(item))
                    {
                        points.Remove(item);
                    }
                }
            }
        } 
        #endregion
    }
}