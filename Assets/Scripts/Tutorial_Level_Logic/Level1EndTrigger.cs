using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1EndTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyController[] enemies; 
    [SerializeField] private Animator doorAnimator; 
    [SerializeField] private GameObject levelCompletePanel; 

    private bool _doorOpened = false;
    private bool _levelCompleted = false;

    private void Update()
    {
        //check if all enemies are dead
        if (!_doorOpened && AreAllEnemiesDead())
        {
            OpenDoor();
        }
    }

    private bool AreAllEnemiesDead()
    {
        foreach (var enemy in enemies)
        {
            if (!enemy._isDead) return false;
        }
        return true;
    }

    private void OpenDoor()
    {
        _doorOpened = true;
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("canProceed");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_levelCompleted) return;

        if (other.CompareTag("Player") && _doorOpened)
        {
            _levelCompleted = true;
            CompleteLevel();
        }
    }

    private void CompleteLevel()
    {
        // Pause the game and show panel 
        Time.timeScale = 0f;
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
        }

        // Switch to UI action map
        InputManager.instance.playerControls.UI.Enable();
        InputManager.instance.playerControls.Movement.Disable();
    }
}
