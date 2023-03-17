using Gameplay.Space.Generator;
using Gameplay.Space.SpaceObjects.Scriptables;
using UnityEngine;

namespace DebugLevelGenerator.Editor
{
    public sealed class DebugLevelMap
    {
        private readonly DebugLevelGeneratorView _debugLevelGeneratorView;

        public DebugLevelMap(DebugLevelGeneratorView debugLevelGeneratorView, SpaceConfig spaceConfig, int[,] borderMap, int[,] nebulaMap)
        {
            _debugLevelGeneratorView = debugLevelGeneratorView;

            new LevelMap(_debugLevelGeneratorView.SpaceView, spaceConfig, borderMap, nebulaMap).Draw();
        }

        public void SetSpaceObjectsTile(Vector3Int positionTile)
        {
            _debugLevelGeneratorView.SpaceObjectsTilemap.SetTile(positionTile, _debugLevelGeneratorView.SpaceObjectsTileBase);
        }
    }
}