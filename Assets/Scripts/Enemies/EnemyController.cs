using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    #region Variables
    [Header("Enemy Movement Properties")]
    public Transform pointA;
    public Transform pointB;
    public float hopForceX = 3f;
    public float hopForceY = 5f;
    public float hopDelay = 0.5f;

    [Header("Ground Check Properties")]
    public Transform groundCheck;
    public float checkRadius = 0.1f;
    public LayerMask _groundLayer;

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
    private bool _isKnockedBack = false;

    [Header("Damage Properties")]
    public int damage = 1;
    public float damageCooldown = 0.5f;
    private bool _canDamage = true;

    private Rigidbody2D _rb;
    private bool _movingToB = false;
    private bool _isDead = false;
    #endregion

    #region Start
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        //ensure gravity is on
        _rb.gravityScale = 2f;

        currentHealth = maxHealth;
        originalColor = spriteRenderer.color;

        StartCoroutine(EnemyBounce());
    }
    #endregion

    #region Method/Functions

    #region Bounce Patrol Logic
    private IEnumerator EnemyBounce()
    {
        while (!_isDead)
        {
            //skip if in knockback
            if (_isKnockedBack)
            {
                yield return null;
                continue;
            }

            //wait until grounded
            yield return new WaitUntil(() => IsGrounded());
            //small pause
            yield return new WaitForSeconds(hopDelay);

            //calculate direction and face the next direction
            Vector2 targetPos = _movingToB ? pointB.position : pointA.position;
            spriteRenderer.flipX = targetPos.x > transform.position.x;

            //reset horizontal velocity
            _rb.velocity = new Vector2(0, _rb.velocity.y);

            //calculate hop distance
            float distanceX = targetPos.x - transform.position.x;
            Vector2 hopForce = new Vector2(distanceX, hopForceY);

            //apply hop force
            _rb.AddForce(hopForce, ForceMode2D.Impulse);

            //flip target for next hope
            _movingToB = !_movingToB;
        }

    }
    private bool IsGrounded()
    {
        bool onGround = Physics2D.OverlapCircle(groundCheck.position, checkRadius, _groundLayer);
        bool fallingOrStill = _rb.velocity.y <= 0.01f;
        return onGround && fallingOrStill;

    }
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
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
        StartCoroutine(FlashRed());

        //start knockback
        StartCoroutine(HandleKnockback(hitDirection));

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator HandleKnockback(Vector2 dir)
    {
        _isKnockedBack = true;

        //reset vertical velocity to avoid stacking with jump
        _rb.velocity = Vector2.zero;

        //Apply knockback force
        _rb.AddForce(dir * knockBackForce, ForceMode2D.Impulse);

        //Wait for knockback duration
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
        if (_isDead || !_canDamage)
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
