using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

namespace Gameplay.Space.Generator
{
    public sealed class SpawnPointsFinder
    {
        private const byte MaxCountSpawnTries = 10;

        private readonly int[,] _map;
        private readonly Tilemap _tilemap;
        private readonly int[,] _spaceObjectsMap;
        private readonly float _maxRealCellSize;

        private readonly List<Point> _availablePoints = new();

        public SpawnPointsFinder(int[,] map, Tilemap tilemap)
        {
            _map = map;
            _tilemap = tilemap;
            _spaceObjectsMap = new int[_map.GetLength(0), _map.GetLength(1)];

            _maxRealCellSize = Mathf.Max(
                _tilemap.cellSize.x * _tilemap.transform.localScale.x, 
                _tilemap.cellSize.y * _tilemap.transform.localScale.y);
            
            _availablePoints = GetEmptyPoints(map);
        }

        public bool TryGetSpaceObjectSpawnPoint(float starSize, float orbit, out Vector3 spawnPoint)
        {
            var occupiedRadius = starSize / 2 + orbit;
            return TryGetSpawnPoint(CellType.SpaceObjects, occupiedRadius, out spawnPoint);
        }

        public Vector3 GetPlayerSpawnPoint()
        {
            TryFindAvailablePointOnMap(_availablePoints, CellType.Player, _spaceObjectsMap, 1, 1);
            return GetSpawnPoint(CellType.Player);
        }

        public bool TryGetEnemySpawnPoint(int groupCount, out Vector3 spawnPoint)
        {
            var occupiedRadius = groupCount * 2;
            return TryGetSpawnPoint(CellType.Enemy, occupiedRadius, out spawnPoint);
        }

        private List<Point> GetEmptyPoints(int[,] map)
        {
            var points = new List<Point>();

            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    if (map[x, y] == (int)CellType.None)
                    {
                        points.Add(new Point(x, y));
                    }
                }
            }

            return points;
        }

        private bool TryGetSpawnPoint(CellType cellType, float occupiedRadius, out Vector3 spawnPoint)
        {
            spawnPoint = default;
            var correctOccupiedRadius = CalculateCorrectOccupiedRadius(occupiedRadius);
            var isFound = TryFindAvailablePointOnMap(_availablePoints, cellType, _spaceObjectsMap, 1, correctOccupiedRadius);

            if (isFound)
            {
                spawnPoint = GetSpawnPoint(cellType);
                Debug.Log($"{cellType}Radius: {Mathf.CeilToInt(correctOccupiedRadius)}");
            }

            return isFound;
        }

        private int CalculateCorrectOccupiedRadius(float radius)
        {
            var occupiedRadius = radius / _maxRealCellSize;
            return Mathf.CeilToInt(occupiedRadius);
        }

        private bool TryFindAvailablePointOnMap(List<Point> points, CellType cellType, int[,] map, int cellCount, int radius)
        {
            var count = 0;
            var tryCount = 0;
            var pseudoRandom = new Random();
            
            while (count < cellCount)
            {
                if (points.Count == 0 || tryCount > MaxCountSpawnTries)
                {
                    Debug.LogWarning($"Not enough space for all \"{cellType}\" | Count = {count}");
                    return false;
                }

                var i = pseudoRandom.Next(points.Count);

                if (СheckNeighborsInRadius(points, points[i].X, points[i].Y, radius))
                {
                    tryCount++;
                    continue;
                }

                map[points[i].X, points[i].Y] = (int)cellType;
                RemovePoints(points, points[i].X, points[i].Y, radius);
                count++;
                tryCount = 0;
            }

            return true;
        }

        private bool СheckNeighborsInRadius(List<Point> points, int gridX, int gridY, int radius)
        {
            var squareRadius = radius * radius;
            for (int neighbourX = gridX - radius; neighbourX <= gridX + radius; neighbourX++)
            {
                for (int neighbourY = gridY - radius; neighbourY <= gridY + radius; neighbourY++)
                {
                    var circleX = neighbourX - gridX;
                    var circleY = neighbourY - gridY;
                    var value = circleX * circleX + circleY * circleY;

                    if (value <= squareRadius)
                    {
                        if (!points.Contains(new(neighbourX, neighbourY)))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void RemovePoints(List<Point> points, int gridX, int gridY, int radius)
        {
            var squareRadius = radius * radius;
            for (int neighbourX = gridX - radius; neighbourX <= gridX + radius; neighbourX++)
            {
                for (int neighbourY = gridY - radius; neighbourY <= gridY + radius; neighbourY++)
                {
                    var circleX = neighbourX - gridX;
                    var circleY = neighbourY - gridY;
                    var value = circleX * circleX + circleY * circleY;

                    if (value <= squareRadius)
                    {
                        var item = new Point(neighbourX, neighbourY);

                        if (points.Contains(item))
                        {
                            points.Remove(item);
                        }
                    }
                }
            }
        }

        private Vector3 GetSpawnPoint(CellType cellType)
        {
            var startX = -_spaceObjectsMap.GetLength(0) / 2;
            var startY = -_spaceObjectsMap.GetLength(1) / 2;

            for (int x = 0; x < _spaceObjectsMap.GetLength(0); x++)
            {
                for (int y = 0; y < _spaceObjectsMap.GetLength(1); y++)
                {
                    if (_spaceObjectsMap[x, y] == (int)cellType)
                    {
                        var positionTile = new Vector3Int(startX + x, startY + y, 0);
                        _spaceObjectsMap[x, y] = (int)CellType.Spawned;
                        return _tilemap.GetCellCenterWorld(positionTile);
                    }
                }
            }

            Debug.LogWarning("Zero position");
            return new();
        }
    }
}