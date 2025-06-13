using UnityEngine;
using System;
using System.Collections.Generic;

public class GameManager : Singleton<GameManager>
{

    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        Win,
        Lose
    }

    private GameState currentState;
    public GameState CurrentState
    {
        get => currentState;
        private set
        {
            if (currentState != value)
            {
                currentState = value;
                OnGameStateChanged?.Invoke(currentState);
            }
        }
    }

    public event Action<GameState> OnGameStateChanged;

    // Dictionary to store enter and exit actions for each state
    private Dictionary<GameState, Action> enterStateActions;
    private Dictionary<GameState, Action> exitStateActions;

    private void InitializeStateActions()
    {
        // Initialize enter state actions
        enterStateActions = new Dictionary<GameState, Action>
        {
            { GameState.MainMenu, () => { Time.timeScale = 1f; /* Add main menu logic */ } },
            { GameState.Playing, () => { Time.timeScale = 1f; /* Add playing logic */ } },
            { GameState.Paused, () => { Time.timeScale = 0f; /* Add pause logic */ } },
            { GameState.Win, () => { Time.timeScale = 0f; /* Add win logic */ } },
            { GameState.Lose, () => { Time.timeScale = 0f; /* Add lose logic */ } }
        };

        // Initialize exit state actions
        exitStateActions = new Dictionary<GameState, Action>
        {
            { GameState.MainMenu, () => { /* Add main menu cleanup */ } },
            { GameState.Playing, () => { /* Add playing cleanup */ } },
            { GameState.Paused, () => { /* Add pause cleanup */ } },
            { GameState.Win, () => { /* Add win cleanup */ } },
            { GameState.Lose, () => { /* Add lose cleanup */ } }
        };
    }

    protected override void Awake()
    {
        base.Awake();
        InitializeStateActions();
    }

    private void Start()
    {
        // Initialize game with MainMenu state
        ChangeState(GameState.Playing);

        // Subscribe to events
        EventManager.Instance.StartListening(EventName.TogglePause, OnTogglePause);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        // Unsubscribe from events
        EventManager.Instance.StopListening(EventName.TogglePause, OnTogglePause);
    }

    private void OnTogglePause(Dictionary<string, object> message)
    {
        if (CurrentState == GameState.Playing)
        {
            PauseGame();
        }
        else if (CurrentState == GameState.Paused)
        {
            ResumeGame();
        }
    }

    public void ChangeState(GameState newState)
    {
        // Exit current state
        if (exitStateActions.TryGetValue(CurrentState, out Action exitAction))
        {
            exitAction?.Invoke();
        }
        else{
            Debug.LogWarning("No exit action found for state: " + CurrentState);
        }

        // Enter new state
        if (enterStateActions.TryGetValue(newState, out Action enterAction))
        {
            enterAction?.Invoke();
        }

        CurrentState = newState;

        // Trigger appropriate event
        switch (newState)
        {
            case GameState.Paused:
                EventManager.Instance.TriggerEvent(EventName.GamePaused, null);
                break;
            case GameState.Playing:
                EventManager.Instance.TriggerEvent(EventName.GameResumed, null);
                break;
        }
    }

    // Public methods to change states
    public void StartGame()
    {
        ChangeState(GameState.Playing);
    }

    public void PauseGame()
    {
        if (CurrentState == GameState.Playing)
        {
            ChangeState(GameState.Paused);
        }
    }

    public void ResumeGame()
    {
        if (CurrentState == GameState.Paused)
        {
            ChangeState(GameState.Playing);
        }
    }

    public void GameWin()
    {
        ChangeState(GameState.Win);
    }

    public void GameLose()
    {
        ChangeState(GameState.Lose);
    }

    public void ReturnToMainMenu()
    {
        ChangeState(GameState.MainMenu);
    }
} 