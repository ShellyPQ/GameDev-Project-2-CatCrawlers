using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogTongueAttack : MonoBehaviour
{
    #region Variables

    [Header("Attack Radius Properties")]
    public float attackRadius = 3f;
    public LayerMask playerMask;
    public LayerMask wallMask;

    [Header("Tongue")]
    public Transform mouthPoint;
    public float maxTongueLength = 2f;
    public float extendSpeed = 8f;
    public float retractSpeed = 12f;
    public float tongueTipRadius = 0.12f;

    [Header("Attack Settings")]
    public float attackCooldown = 2f;
    private bool canAttack = true;
    private bool attacking = false;
    private float currentLength = 0f;

    private SpriteRenderer _spriteRenderer;
    private Animator _ani;
    private EnemyController _enemyController;
    private Vector2 _attackDir;

    #endregion

    #region Awake
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _ani = GetComponent<Animator>();
        _enemyController = GetComponent<EnemyController>();
    }
    #endregion

    #region Update
    private void Update()
    {
        if (_enemyController._isDead || _enemyController._isKnockedBack || _enemyController._pauseAI) return;

        if (!attacking && canAttack && PlayerAboveInRange())
        {
            StartTongueAnimation();
        }

        if (attacking)
        {
            HandleTongue();
        }
    }
    #endregion

    #region Method/Functions

    private bool PlayerAboveInRange()
    {
        Vector2 origin = transform.position + Vector3.up * 0.5f;
        Collider2D hit = Physics2D.OverlapCircle(origin, attackRadius, playerMask);

        if (hit == null) return false;

        //player must be above the frog
        //if (hit.transform.position.y > transform.position.y + 0.1f) return true;

        return true;
    }

    private void StartTongueAnimation()
    {
        canAttack = false;

        Transform player = PlayerHealth.instance.transform;

        bool playerAbove = player.position.y > transform.position.y + 0.1f;
        bool playerBeside = Mathf.Abs(player.position.x - transform.position.x) < 0.5f;

        if (playerAbove && playerBeside)
        {
            _attackDir = Vector2.up;
            _ani.SetTrigger("AttackVertical");
        }
        else
        {
            _attackDir = player.position.x > transform.position.x ? Vector2.right : Vector2.left;            
            _spriteRenderer.flipX = _attackDir == Vector2.right;
            _ani.SetTrigger("AttackHorizontal");
        }
    }

    public void StartTongueAttack()
    {
        attacking = true;
        currentLength = 0;

        //place tongue attack sfx here
        //SFXManager.instance.PlaySFX("frogTongueAttack");
    }

    public void EndTongueAttack()
    {
        attacking = false;
        StartCoroutine(AttackCooldown());
    }

    private void HandleTongue()
    {
        //determine tongue direction from stored value
        Vector2 dir = _attackDir;

        //extend until max or until the tongue hits a wall
        float nextLength = currentLength + extendSpeed * Time.deltaTime;

        //check wall before moving tongue
        RaycastHit2D wallHit = Physics2D.Raycast(mouthPoint.position, dir, nextLength, wallMask);

        if (wallHit.collider != null)
        {
            currentLength = wallHit.distance;
            EndAttackEarly();
            return;
        }
        else
        {
            currentLength = nextLength;
        }

        currentLength = Mathf.Clamp(currentLength, 0, maxTongueLength);

        //position of the tip of the tongue
        Vector3 tongueTipPOS = mouthPoint.position + (Vector3)(dir * currentLength);

        //check if tongue has hit the player
        Collider2D playerHit = Physics2D.OverlapCircle(tongueTipPOS, tongueTipRadius, playerMask);

        if (playerHit != null)
        {
            PlayerHealth player = playerHit.GetComponent<PlayerHealth>();
            if (player != null)
            {
                Vector2 knockbackDir = (player.transform.position - transform.position).normalized;
                player.TakeDamage(1, knockbackDir, 5f);

                attacking = false;
                currentLength = 0f;
                StartCoroutine(AttackCooldown());
                return;
            }
        }

        //if fully extended and not hitting wall or player, retract length
        if (currentLength >= maxTongueLength)
        {
            currentLength -= retractSpeed * Time.deltaTime;
        }

        //if fully retracted, stop attack
        if (currentLength <= 0)
        {
            attacking = false;
            StartCoroutine(AttackCooldown());
        }

        Debug.DrawRay(mouthPoint.position, dir * currentLength, Color.red);
    }

    private void EndAttackEarly()
    {
        attacking = false;
        currentLength = 0f;

        //reset animator state so attack stops
        _ani.ResetTrigger("AttackHorizontal");
        _ani.ResetTrigger("AttackVertical");

        _ani.Play("Idle", 0, 0);
        StartCoroutine(AttackCooldown());
    }
    #endregion

    #region Coroutine

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.5f, attackRadius);
    }
    #endregion
}
