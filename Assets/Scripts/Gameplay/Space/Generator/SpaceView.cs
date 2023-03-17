using Gameplay.Space.Obstacle;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Gameplay.Space.Generator
{
    public sealed class SpaceView : MonoBehaviour
    {
        [field: SerializeField, Header("Border")] public Tilemap BorderTilemap { get; private set; }
        [field: SerializeField] public Tilemap BorderMaskTilemap { get; private set; }

        [field: SerializeField, Header("Nebula")] public Tilemap NebulaTilemap { get; private set; }
        [field: SerializeField] public Tilemap NebulaMaskTilemap { get; private set; }

        [field: SerializeField, Header("Obstacle")] public SpaceObstacleView SpaceObstacleView { get; private set; }
    }
}