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
    public Transform bossRespawnPoint;
    public bool inBossFight = false;

    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private Animator _ani;

    private bool _isDead = false;
    private bool _isKnockedBack = false;
    private bool _invulnerable = false;
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

        if (RespawnManager.respawnAtBoss)
        {
            transform.position = bossRespawnPoint.position;
            RespawnManager.respawnAtBoss = false;
        }
    }
    #endregion

    #region Method/Functions
    public void TakeDamage(int dmg, Vector2 knockbackDir, float knockbackForce)
    {
        if (_isDead || _invulnerable)
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

    public bool SetInvulnerable(bool value)
    {
        _invulnerable = value;
        
        return _invulnerable;
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

        _spriteRenderer.color = original;
    }

    //knockback coroutine when the player is hit
    private IEnumerator KnockbackRoutine()
    {
        _ani.SetBool("isHurt", true);
        _isKnockedBack = true;
        yield return new WaitForSeconds(0.2f);
        _isKnockedBack = false;
        _ani.SetBool("isHurt", false);
    }

    //when the player is hit
    public bool IsKnockedBack() => _isKnockedBack;

    //when the player dies
    private void GameOver()
    {
        if (_isDead)
        {
            return;
        }

        _isDead = true;

        _rb.velocity = Vector3.zero;

        if (inBossFight)   // or whatever bool you track
        {
            RespawnManager.respawnAtBoss = true;
            ScenesManager.Instance.RestartLevel();
            return;
        }

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

        //start the trigger ko coroutine, shrink the collider, add delay then call gameover panel
        StartCoroutine(TriggerKOState());
    }

    private IEnumerator TriggerKOState()
    {
        CapsuleCollider2D col = GetComponent<CapsuleCollider2D>();
        if (col == null)
        {
            PauseManager.instance.GameOver();
            yield break;
        }

        //trigger ko animation
        _ani.SetBool("isKO", true);

        //wait for animation to play through before shrinking collider
        yield return new WaitForSecondsRealtime(.45f);

        float duration = 0.4f;
        float elapsed = 0f;

        Vector2 startSize = col.size;
        Vector2 startOffset = col.offset;

        float shrinkAmount = startSize.y * 0.3f;
        Vector2 targetSize = new Vector2(startSize.x, startSize.y - shrinkAmount);
        Vector2 targetOffset = new Vector2(startOffset.x, startOffset.y + (shrinkAmount / 2f));

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; 
            float t = Mathf.Clamp01(elapsed / duration);

            col.size = Vector2.Lerp(startSize, targetSize, t);
            col.offset = Vector2.Lerp(startOffset, targetOffset, t);

            Physics2D.SyncTransforms();
            yield return null;
        }

        col.size = targetSize;
        col.offset = targetOffset;
        Physics2D.SyncTransforms();

        col.enabled = false;
        col.enabled = true;
        Physics2D.SyncTransforms();

        yield return new WaitForSecondsRealtime(0.6f);
        PauseManager.instance.GameOver();
    }

    public void ResetPlayerCollider()
    {
        CapsuleCollider2D col = GetComponent<CapsuleCollider2D>();
        if (col == null)
        {
            return;
        }

        //restore original collide values
        col.direction = CapsuleDirection2D.Vertical;
        Vector2 startSize = new Vector2(2f, 6.88f);
        Vector2 startOffset = new Vector2(0.7f, 0.06f);
    }
    #endregion
}
