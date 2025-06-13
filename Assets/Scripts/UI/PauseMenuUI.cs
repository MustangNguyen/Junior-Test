using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PauseMenuUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button pauseResumeButton;
    [SerializeField] private TextMeshProUGUI pauseResumeButtonText;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button quitButton;

    private bool isPaused = false;

    private void Start()
    {
        // Setup button listeners
        pauseResumeButton.onClick.AddListener(OnPauseResumeButtonClicked);
        quitButton.onClick.AddListener(OnQuitButtonClicked);

        // Subscribe to game events
        EventManager.Instance.StartListening(EventName.GamePaused, OnGamePaused);
        EventManager.Instance.StartListening(EventName.GameResumed, OnGameResumed);

        // Initial UI state
        UpdateUI();
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        EventManager.Instance.StopListening(EventName.GamePaused, OnGamePaused);
        EventManager.Instance.StopListening(EventName.GameResumed, OnGameResumed);
    }

    private void OnPauseResumeButtonClicked()
    {
        // Just trigger the event, let GameManager handle the logic
        EventManager.Instance.TriggerEvent(EventName.TogglePause, null);
    }

    private void OnQuitButtonClicked()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void OnGamePaused(Dictionary<string, object> message)
    {
        isPaused = true;
        UpdateUI();
    }

    private void OnGameResumed(Dictionary<string, object> message)
    {
        isPaused = false;
        UpdateUI();
    }

    private void UpdateUI()
    {
        pausePanel.SetActive(isPaused);
        pauseResumeButtonText.text = isPaused ? "Resume" : "Pause";
    }
} 