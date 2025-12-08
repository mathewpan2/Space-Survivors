using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class GameManager : MonoBehaviour
{
    [Header("Spawning / Survival")]
    public EnemySpawner enemySpawner;
    public float survivalTime = 120f;
    private float remainingTime;
    public TextMeshProUGUI timerText;

    [Header("Victory UI")]
    public VictoryUI victoryUI;

    [Header("Pause Menu")]
    public GameObject pauseMenuUI;   // assign your Pause Panel here in Inspector

    public static bool GameIsPaused = false;
    private bool gameEnded = false;

    // ---------------- TIMER / VICTORY ----------------

    void OnTimerEnd()
    {
        gameEnded = true;
        Debug.Log("Survival Time Over!");
        timerText.text = "0:00";
        enemySpawner.StopSpawning();

        // make sure pause menu is hidden if it was open
        if (pauseMenuUI) pauseMenuUI.SetActive(false);
        GameIsPaused = false;

        Time.timeScale = 0f;  // freeze game for victory
        if (victoryUI && victoryUI.victoryPanel) victoryUI.showUI();
    }

    // ---------------- UNITY LIFECYCLE ----------------

    void Start()
    {
        Time.timeScale = 1f; // make sure game starts unpaused
        GameIsPaused = false;
        gameEnded = false;

        enemySpawner.StartSpawning();
        remainingTime = survivalTime;
    }

    void Update()
    {
        HandlePauseInput();
        HandleTimer();
    }

    // ---------------- PAUSE LOGIC ----------------

    void HandlePauseInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Don't allow pausing/unpausing after game over
            if (gameEnded) return;

            if (GameIsPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void ResumeGame()
    {
        if (pauseMenuUI) pauseMenuUI.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void PauseGame()
    {
        if (pauseMenuUI) pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
        // or load main menu scene here instead if you have one
        // SceneManager.LoadScene("MainMenu");
    }

    // ---------------- TIMER LOGIC ----------------

    void HandleTimer()
    {
        if (gameEnded) return;           // stop once time is over
        if (GameIsPaused) return;        // don't tick down while paused

        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;

            // Clamp if it crosses below zero
            if (remainingTime <= 0)
            {
                remainingTime = 0;
                UpdateTimerText();
                OnTimerEnd();
                return;
            }

            UpdateTimerText();
        }
    }

    void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0}:{1:00}", minutes, seconds);
    }
}
