using Abstracts;
using Gameplay.GameState;
using UnityEngine;

namespace UI.MainMenu
{
    public sealed class MainMenuController : BaseController
    {
        private readonly CurrentState _currentState;
        private readonly Canvas _mainUICanvas;

        public MainMenuController(CurrentState currentState, Canvas mainUICanvas)
        {
            _currentState = currentState;
            _mainUICanvas = mainUICanvas;
            
            //TODO Menu load
            
            //TODO Remake when menu will be done
            _currentState.CurrentGameState.Value = GameState.Game;
        }
    }
}