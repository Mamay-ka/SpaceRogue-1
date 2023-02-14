using Abstracts;
using Gameplay;
using Gameplay.GameState;
using UI;
using UI.MainMenu;
using UnityEngine;


public sealed class MainController : BaseController
{
    private readonly CurrentState _currentState;
    private readonly MainUIController _mainUIController;

    private GameController _gameController;
    private MainMenuController _mainMenuController;
    

    public MainController(CurrentState currentState, Transform uiPosition)
    {
        _currentState = currentState;

        _mainUIController = new MainUIController(uiPosition);
        AddController(_mainUIController);
        
        _currentState.CurrentGameState.Subscribe(OnGameStateChange);
        OnGameStateChange(_currentState.CurrentGameState.Value);
    }

    protected override void OnDispose()
    {
        DisposeAllControllers();
        
        _currentState.CurrentGameState.Unsubscribe(OnGameStateChange);
    }

    private void OnGameStateChange(GameState newState)
    {
        DisposeAllControllers();
        
        switch (newState)
        {
            case GameState.Menu:
                _mainMenuController = new MainMenuController(_currentState, _mainUIController.MainCanvas);
                break;
            case GameState.Game:
                _gameController = new GameController(_currentState, _mainUIController.MainCanvas);
                break;
            case GameState.None:
            default: break;
        }
    }

    private void DisposeAllControllers()
    {
        _gameController?.Dispose();
        _mainMenuController?.Dispose();
    }
    
}