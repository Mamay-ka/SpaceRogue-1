using UnityEngine;

namespace UI.Game
{
    public sealed class MainCanvasView : MonoBehaviour
    {
        [field: SerializeField] public Transform PlayerInfo { get; private set; }
        [field: SerializeField] public Transform EnemyHealthBars { get; private set; }
        [field: SerializeField] public Transform GameEventIndicators { get; private set; }
    }
}