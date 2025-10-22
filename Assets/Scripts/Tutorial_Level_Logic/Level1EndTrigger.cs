using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1EndTrigger : MonoBehaviour
{
    //subscribe to challenge event
    public static event System.Action OnLevelCompleted;

    #region Variables

    [Header("References")]
    [SerializeField] private EnemyController[] enemies; 
    [SerializeField] private Animator doorAnimator; 
    [SerializeField] private GameObject levelCompletePanel; 

    private bool _doorOpened = false;
    private bool _levelCompleted = false;

    #endregion

    #region Update
    private void Update()
    {
        //check if all enemies are dead
        if (!_doorOpened && AreAllEnemiesDead())
        {
            OpenDoor();
        }
    }
    #endregion

    #region Method/Functions
    //check to see if all enemies are dead
    private bool AreAllEnemiesDead()
    {
        foreach (var enemy in enemies)
        {
            if (!enemy._isDead) return false;
        }
        return true;
    }

    //trigger door animation
    private void OpenDoor()
    {
        _doorOpened = true;
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("canProceed");
            // Play gate open SFX
            SFXManager.instance.playSFX("doorUnlock");
        }
    }

    //when entering the end level trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_levelCompleted) return;

        if (other.CompareTag("Player") && _doorOpened)
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
