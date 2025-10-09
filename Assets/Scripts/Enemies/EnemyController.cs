using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, IDamageable
{
    #region Variables 
    [Header("Enemy Reference")]
    [Tooltip("Assign the enemies AI")]
    [SerializeField] GameObject _enemyAI;
    [SerializeField] EnemyStunEffect _enemyStunEffect;

    [Header("Enemy Stats")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Visual Properties")]
    public SpriteRenderer spriteRenderer;
    public Sprite catSprite;
    private Color originalColor;

    [Header("Knockback")]
    public float knockBackForce = 5f;
    public float knockBackDuration = 0.2f;
    [HideInInspector] public bool _isKnockedBack = false;
    [HideInInspector] public bool _pauseAI = false;

    [Header("Damage Properties")]
    public bool canDamagePlayer = false;
    public int damage = 1;
    public float damageCooldown = 0.5f;
    private bool _canDamage = true;

    [HideInInspector] public Rigidbody2D _rb;    
    public bool _isDead = false;

    [Header("Ground Check Properties")]
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _checkRadius = 0.1f;
    [SerializeField] private LayerMask _groundLayer;

    #endregion

    #region Awake
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        //stop enemy from rotating when hit
        _rb.freezeRotation = true; 
    }
    #endregion

    #region Start
    private void Start()
    {        
        //ensure gravity is on
        _rb.gravityScale = 2f;

        currentHealth = maxHealth;
        originalColor = spriteRenderer.color;        
    }
    #endregion

    #region Method/Functions    

    #region Enemy Ground Check
    public bool IsGrounded()
    {
        bool onGround = Physics2D.OverlapCircle(_groundCheck.position, _checkRadius, _groundLayer);
        bool fallingOrStill = _rb.velocity.y <= 0.01f;

        return onGround && fallingOrStill;
    }

    private void OnDrawGizmosSelected()
    {
        if (_groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_groundCheck.position, _checkRadius);
        }
    }
    #endregion

    #region Damage & Death Logic
    public void TakeDamage(int dmg, Vector2 hitDirection)
    {
        if (_isDead)
        {
            return;
        }

        currentHealth -= dmg;

        SFXManager.instance.playSFX("enemyHurt");

        StartCoroutine(FlashRed());
        //start knockback
        StartCoroutine(EnemyKnockback(hitDirection));

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator EnemyKnockback(Vector2 dir)
    {
        _isKnockedBack = true;

        //reset vertical velocity to avoid stacking with jump
        _rb.velocity = Vector2.zero;

        //apply knockback force
        _rb.AddForce(dir * knockBackForce, ForceMode2D.Impulse);

        //wait for knockback duration
        yield return new WaitForSeconds(knockBackDuration);

        //wait until grounded to resume bouncing
        yield return new WaitUntil(() => IsGrounded());

        _isKnockedBack = false;
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        if (!_isDead)
        {
            spriteRenderer.color = originalColor;
        }        
    }

    public void Stun(float duration)
    {
        if (_isDead)
        {
            return;
        }

        //pause the enemy AI
        _pauseAI = true;
        //stop movement
        _rb.velocity = Vector2.zero;

        if (_enemyStunEffect != null)
        {
            //triggers squeeze + twist
            _enemyStunEffect.Stun();
        }
        StartCoroutine(StunCoroutine(duration));
    }

    private IEnumerator StunCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        //resume movement/hops
        _pauseAI = false;
    }

    private void Die()
    {
        _isDead = true;
        StopAllCoroutines();

        //stop bounce AI logic but keep gravity
        _isKnockedBack = false;

        //Keep gravity, but kill sideways movement
        _rb.velocity = Vector2.zero;
        _rb.isKinematic = false;
        _rb.gravityScale = 2f;       

        //Reset color and swap sprite
        spriteRenderer.color = originalColor;
        spriteRenderer.sprite = catSprite;

        //move behind player visually
        spriteRenderer.sortingOrder -= 1;

        //disable the enemys current collider 
        Collider2D enemyCol = GetComponent<Collider2D>();
        if (enemyCol != null)
        {
            enemyCol.enabled = false;
        }

        //add a new collider for corpse physics
        BoxCollider2D corpseCol = gameObject.AddComponent<BoxCollider2D>();
        corpseCol.isTrigger = false;
        corpseCol.size = new Vector2(spriteRenderer.bounds.size.x * 0.8f, 0.2f);

        //position collider so its bottom sits at y = 0;
        float spriteBottom = spriteRenderer.bounds.min.y - transform.position.y;
        corpseCol.offset = new Vector2(0f, spriteBottom + corpseCol.size.y / 2f);
        gameObject.layer = LayerMask.NameToLayer("Corpse");       
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!canDamagePlayer || _isDead || !_canDamage)
        {
            return;
        }

        if (other.gameObject.tag == "Player")
        {   
            PlayerHealth player = other.gameObject.GetComponent<PlayerHealth>();
            if (player != null)
            {
                //direction from enemy to player
                Vector2 knockbackDir = (other.transform.position - transform.position).normalized;

                float playerKnockbackForce = 5f;
                player.TakeDamage(damage, knockbackDir, playerKnockbackForce);

                StartCoroutine(DamageCooldown());
            }
        }
    }

    private IEnumerator DamageCooldown()
    {
        _canDamage = false;
        yield return new WaitForSeconds(damageCooldown);
        _canDamage = true;
    }
    #endregion

    #endregion
}
