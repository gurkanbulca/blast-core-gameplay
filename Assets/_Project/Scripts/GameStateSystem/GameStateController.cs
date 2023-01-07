using System;
using UnityEngine;

namespace GameStateSystem
{
    public class GameStateController
    {
        #region Actions

        public static event Action<GameState> OnCurrentGameStateChanged = delegate { };

        #endregion

        #region Constructor

        public GameStateController(GameState initialState)
        {
            currentGameState = initialState;
        }

        #endregion

        #region Private Fields

        private GameState _currentGameState;

        #endregion

        #region Properties

        public GameState currentGameState
        {
            get => _currentGameState;
            set
            {
                if (value == _currentGameState)
                {
                    return;
                }


                _currentGameState = value;
                Debug.Log($"Current game state changed to: {value.ToString()}");
                OnCurrentGameStateChanged(value);
            }
        }

        #endregion
    }
}