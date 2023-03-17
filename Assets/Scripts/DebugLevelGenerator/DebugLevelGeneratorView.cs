using Gameplay.Space.Generator;
using Gameplay.Space.SpaceObjects.Scriptables;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace DebugLevelGenerator
{
    public sealed class DebugLevelGeneratorView : MonoBehaviour
    {
        [field: SerializeField, Header("Settings")] public SpaceView SpaceView { get; private set; }
        [field: SerializeField] public SpaceConfig SpaceConfig { get; private set; }
        [field: SerializeField] public SpaceObjectSpawnConfig SpaceObjectSpawnConfig { get; private set; }

        [field: SerializeField, Header("SpaceObjects")] public Tilemap SpaceObjectsTilemap { get; private set; }
        [field: SerializeField ] public TileBase SpaceObjectsTileBase { get; private set; }
    }
}