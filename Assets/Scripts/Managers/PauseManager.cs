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

    #region Update
    private void Update()
    {
        CheckPauseState();
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

        InputManager.playerInput.SwitchCurrentActionMap("UI");
    }

    public void UnpauseGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        InputManager.playerInput.SwitchCurrentActionMap("Movement");
    }
    #endregion
}
