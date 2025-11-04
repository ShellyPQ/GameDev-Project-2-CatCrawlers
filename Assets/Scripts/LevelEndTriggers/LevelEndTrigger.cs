using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEndTrigger : MonoBehaviour
{
    //subscribe to challenge event
    public static event System.Action OnLevelCompleted;

    #region Variables

    [Header("References")]
    [SerializeField] private GameObject levelCompletePanel;

    //private bool _doorOpened = false;
    private bool _levelCompleted = false;

    #endregion

    #region Method/Functions

    //when entering the end level trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_levelCompleted) return;

        Debug.Log("Level complete");

        if (other.CompareTag("Player"))
        {
            _levelCompleted = true;
            CompleteLevel();
        }
    }

    //when this is called, the level is now complete
    private void CompleteLevel()
    {
        //save level completion
        SaveManager.SaveLevelComplete(1);
        PlayerPrefs.Save();
        _levelCompleted = true;

        //trigger challenge event
        OnLevelCompleted?.Invoke();

        // Pause the game and show panel 
        Time.timeScale = 0f;
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
        }

        // Switch to UI action map
        InputManager.instance.EnableUI();
    }

    #endregion
}
