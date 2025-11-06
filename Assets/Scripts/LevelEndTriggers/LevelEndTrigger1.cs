using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2EndTrigger : MonoBehaviour
{
    //subscribe to challenge event
    public static event System.Action OnLevel2Completed;

    #region Variables

    [Header("References")]
    [SerializeField] private GameObject levelCompletePanel;

    //private bool _doorOpened = false;
    private bool _level2Completed = false;

    #endregion

    #region Method/Functions

    //when entering the end level trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_level2Completed) return;

        Debug.Log("Level complete");

        if (other.CompareTag("Player"))
        {
            _level2Completed = true;
            CompleteLevel();
        }
    }

    //when this is called, the level is now complete
    private void CompleteLevel()
    {
        //save level completion
        SaveManager.SaveLevelComplete(2);
        PlayerPrefs.Save();
        _level2Completed = true;

        //trigger challenge event
        OnLevel2Completed?.Invoke();

        Debug.Log("Opening level panel. Panel reference: " + levelCompletePanel);
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
