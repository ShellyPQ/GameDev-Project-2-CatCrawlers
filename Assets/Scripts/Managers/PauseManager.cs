using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public static PauseManager instance;

    #region Variables
    [Header("References")]
    [Tooltip("Pause Panel Reference")]
    public GameObject pausePanel;
    public GameObject gameOverPanel;

    public bool isPaused { get; private set; }
    #endregion

    #region Awake
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    #endregion

    #region Start
    private void Start()
    {
        //ensure the pause panel is disabled on start
        pausePanel.gameObject.SetActive(false);
        isPaused = false;
    }
    #endregion

    #region Update
    private void Update()
    {
        CheckPauseState();      

        if ( isPaused)
        {

        }
    }
    #endregion

    #region Method/Functions

    private void CheckPauseState()
    {
        if (InputManager.instance.pauseMenuOpen)
        {
            if (!isPaused)
            {
                PauseGame();
            }
        }
        else if (InputManager.instance.pauseMenuClose)
        {
            if (isPaused)
            {
                UnpauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pausePanel.SetActive(true);

        //switch to UI action map
        InputManager.instance.EnableUI();
    }

    public void UnpauseGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pausePanel.SetActive(false);

        //switch back to Movement action map
        InputManager.instance.EnableMovement();
    }

    public void GameOver()
    {
        //pause the game
        Time.timeScale = 0f;

        //show the Game Over panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }           

        //ensure UI input is enabled
        InputManager.instance.EnableUI();
    }
    #endregion
}
