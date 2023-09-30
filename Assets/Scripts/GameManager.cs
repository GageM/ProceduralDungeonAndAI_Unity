using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { NullState, Intro, MainMenu, Game, GameOver };
public delegate void OnStateChangeHandler();

public class GameManager
{
    static GameManager _instance = null; 
    public event OnStateChangeHandler OnStateChange;
    public GameState gameState { get; private set; }
    protected GameManager() { }

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameManager();
            }
            return _instance;
        }
    }

    public void SetGameState(GameState gameState)
    {
        this.gameState = gameState;
        if(OnStateChange != null)
        {
            OnStateChange();
        }

        if (this.gameState == GameState.GameOver)
        {
            Debug.Log("Quitting Game");
            Application.Quit();
        }
    }
}
