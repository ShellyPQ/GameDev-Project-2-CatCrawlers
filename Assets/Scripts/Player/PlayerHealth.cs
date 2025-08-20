using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    #region Variables
    [Header("Properties")]
    [Tooltip("Player has 9 hits")]
    public int maxLives = 9;
    private int _currentLives;

    private Rigidbody2D _rb;
    #endregion

    private void Start()
    {
        _currentLives = maxLives;
        _rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int dmg, Vector2 knockbackDir, float knockbackForce)
    {
        _currentLives -= dmg;
        Debug.Log($"Player hit! Lives remaining:{_currentLives}");
        // TODO: Display lives on UI text element

        //Apply horizontal knockback only
        Vector2 horizontalKnockback = new Vector3(_rb.velocity.x, 0).normalized * knockbackForce;
        _rb.AddForce(horizontalKnockback, ForceMode2D.Impulse);


        if (_currentLives <= 0)
        {
            GameOver();
        }

    }


    public void GameOver()
    {
        Debug.Log("Game Over!");
        // TODO: Add respawn, scene reload, game over UI trigger
    }
}
