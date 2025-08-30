using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth instance;

    #region Variables
    [Header("Properties")]
    [Tooltip("Player has 9 hits")]
    public int maxLives = 9;
    private int _currentLives;

    [Header("References")]
    [Tooltip("Text to display players current lives")]
    public TextMeshProUGUI _currentLivesText;

    private Rigidbody2D _rb;
    #endregion

    #region Awake
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            //make sure there is only one
            Destroy(gameObject);
        }
    }
    #endregion

    #region start
    private void Start()
    {
        //set players health to max health at the start of the level and display in the HUD
        _currentLives = maxLives;
        HUDManager.instance.UpdateRemainingLivesText(_currentLives);

        _rb = GetComponent<Rigidbody2D>();
    }
    #endregion

    public void TakeDamage(int dmg, Vector2 knockbackDir, float knockbackForce)
    {
        //lose a life when taking damage, update HUD to display new health
        _currentLives -= dmg;
        HUDManager.instance.UpdateRemainingLivesText(_currentLives);

        //Apply horizontal knockback only
        Vector2 horizontalKnockback = new Vector3(_rb.velocity.x, 0).normalized * knockbackForce;
        _rb.AddForce(horizontalKnockback, ForceMode2D.Impulse);


        if (_currentLives <= 0)
        {
            GameOver();
        }
        else
        {
            RespawnAtCheckpooint();
        }
    }

    public void RespawnAtCheckpooint()
    {
        Debug.Log("Player is respawning and lost a life");
        //TO DO: move player to checkpoint
    }

    public void GameOver()
    {
        Debug.Log("Game Over!");
        // reset lives when going back to level select
        _currentLives = maxLives;
        //TO DO: Trigger game over panel (coroutine - have it wait then do the below)
        //TO DO: transition to Level Select scene
    }
}
