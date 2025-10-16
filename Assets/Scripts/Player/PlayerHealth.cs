using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth instance;

    #region Variables
    [Header("Properties")]
    public int maxLives = 9;
    private int _currentLives;

    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private Animator _ani;

    private bool _isDead = false;
    private bool _isKnockedBack = false;
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
            Destroy(gameObject);
        }

        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _ani = GetComponent<Animator>();
    }
    #endregion

    #region Start
    private void Start()
    {
        _currentLives = maxLives;
        HUDManager.instance.UpdateRemainingLivesText(_currentLives);
    }
    #endregion

    #region Method/Functions
    public void TakeDamage(int dmg, Vector2 knockbackDir, float knockbackForce)
    {
        if (_isDead) 
        {
            return;
        }

        //clamp lives
        _currentLives = Mathf.Max(_currentLives - dmg, 0);
        HUDManager.instance.UpdateRemainingLivesText(_currentLives);

        _ani.SetBool("isHurt", true);

        SFXManager.instance.playSFX("playerHurt");

        //apply knockback & trigger hurt animation
        if (_rb != null)
        {
            _rb.velocity = Vector2.zero; 
            _rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
            StartCoroutine(KnockbackRoutine());
        }

        StartCoroutine(FlashRed());

        if (_currentLives <= 0)
        {
            //_ani.SetBool("isKO", true);
            GameOver();
        }
    }

    //visual feedback for when the player is hit
    private IEnumerator FlashRed()
    {
        Color original = _spriteRenderer.color;
        Color target = new Color(0.5f, 0f, 0f);

        float elapsed = 0f;
        float duration = 0.2f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _spriteRenderer.color = Color.Lerp(original, target, elapsed / duration);
            yield return null;
        }

        _ani.SetBool("isHurt", false);
        _spriteRenderer.color = original;
    }

    //knockback coroutine when the player is hit
    private IEnumerator KnockbackRoutine()
    {
        _isKnockedBack = true;
        yield return new WaitForSeconds(0.2f); 
        _isKnockedBack = false;
    }

    //when the player is hit
    public bool IsKnockedBack() => _isKnockedBack;

    //when the player dies
    private void GameOver()
    {
        _isDead = true;
        Debug.Log("Game Over!");

        //show game over panel and pause the game
        PauseManager.instance.GameOver(); 
        
        //disable player attack scripts so they cant hit enemies (just to make sure)
        var rangeAttack = GetComponent<RangedAttack>();
        if (rangeAttack != null)
        {
            rangeAttack.enabled = false;
        }

        var meleeAttack = GetComponent<MeleeAttack>();
        if (meleeAttack != null)
        {
            meleeAttack.enabled = false;
        }
    }
    #endregion
}
