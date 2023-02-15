using System;
using Gameplay.GameState;
using UnityEngine;

public sealed class EntryPoint : MonoBehaviour
{
    [SerializeField] private Transform uiPosition;
    [SerializeField] private GameState initialGameState = GameState.Game;

    private MainController _mainController;
    
    private void Awake()
    {
        var gameState = new CurrentState(initialGameState);
        _mainController = new MainController(gameState, uiPosition);
    }
    
    private void OnDestroy()
    {
        _mainController.Dispose();
    }
    
    
    #region UpdateMechanism

    private static event Action OnUpdate = () => { };
    private static event Action<float> OnDeltaTimeUpdate = (_) => { };
    private static event Action OnFixedUpdate = () => { };
    private static event Action<float> OnDeltaTimeFixedUpdate = (_) => { };
    private static event Action OnLateUpdate = () => { };
    private static event Action<float> OnDeltaTimeLateUpdate = (_) => { };
    
    public static void SubscribeToUpdate(Action callback) => OnUpdate += callback;
    public static void UnsubscribeFromUpdate(Action callback) => OnUpdate -= callback;
    public static void SubscribeToUpdate(Action<float> callback) => OnDeltaTimeUpdate += callback;
    public static void UnsubscribeFromUpdate(Action<float> callback) => OnDeltaTimeUpdate -= callback;
    
    public static void SubscribeToFixedUpdate(Action callback) => OnFixedUpdate += callback;
    public static void UnsubscribeFromFixedUpdate(Action callback) => OnFixedUpdate -= callback;    
    public static void SubscribeToFixedUpdate(Action<float> callback) => OnDeltaTimeFixedUpdate += callback;
    public static void UnsubscribeFromFixedUpdate(Action<float> callback) => OnDeltaTimeFixedUpdate -= callback;
    
    public static void SubscribeToLateUpdate(Action callback) => OnLateUpdate += callback;
    public static void UnsubscribeFromLateUpdate(Action callback) => OnLateUpdate -= callback;
    
    private void Update()
    {
        OnUpdate.Invoke();
        OnDeltaTimeUpdate.Invoke(Time.deltaTime);
    }
    private void FixedUpdate()
    {
        OnFixedUpdate.Invoke();
        OnDeltaTimeFixedUpdate.Invoke(Time.fixedDeltaTime);
    }
    private void LateUpdate()
    {
        OnLateUpdate.Invoke();
    }

    #endregion
    
    
}
