using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitchAI : MonoBehaviour
{
    #region Variables

    [Header("Patrol Points")]
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 2f;
    private Vector3 _targetPoint;

    [Header("Attack Properties")]
    public float attackRadius = 4f;
    public float attackCooldown = 1.5f;
    public LayerMask playerMask;
    public Transform firePoint;
    public GameObject projectilePrefab;
    public float projectileForce = 8f;
    private bool _canAttack = true;

    private Animator _ani;
    private EnemyController _enemy;
    private SpriteRenderer _sprite;

    #endregion

    #region Awake
    private void Awake()
    {
        _ani = GetComponent<Animator>();
        _enemy = GetComponent<EnemyController>();
        _sprite = GetComponent<SpriteRenderer>();
        _targetPoint = pointB.position;
    }
    #endregion

    #region Update
    private void Update()
    {
        if (_enemy._isDead || _enemy._isKnockedBack || _enemy._pauseAI) return;

        //check for the player first
        if (_canAttack && PlayerInRange())
        {
            AttackPlayer();
            return;
        }

        Patrol();
    }
    #endregion

    #region Method/Functions

    private void Patrol()
    {
        transform.position = Vector3.MoveTowards(transform.position, _targetPoint, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, _targetPoint) < 0.1f)
        {
            _targetPoint = (_targetPoint == pointA.position) ? pointB.position : pointA.position;
        }

        //flip sprite based on current direction
        _sprite.flipX = _targetPoint.x > transform.position.x;
    }

    private bool PlayerInRange()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, attackRadius, playerMask);

        return hit != null;
    }

    private void AttackPlayer()
    {
        Transform player = PlayerHealth.instance.transform;

        //player direction 
        Vector2 dir = (player.position - transform.position);

        bool playerAbove = dir.y > 0.5f;
        bool playerHorizontal = Mathf.Abs(dir.x) > Mathf.Abs(dir.y);

        if (playerAbove)
        {
            _ani.SetBool("verticalAttack", true);
        }
        else if (playerHorizontal)
        {
            _ani.SetBool("horizontalAttack", true);
            _sprite.flipX = dir.x > 0;
        }

        FireMagic(dir.normalized);
        StartCoroutine(AttackCooldown());
    }

    private void FireMagic(Vector2 dir)
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.velocity = dir * projectileForce;
    }
    #endregion

    #region Coroutine

    private IEnumerator AttackCooldown()
    {
        _canAttack = false;
        yield return new WaitForSeconds(attackCooldown);

        _ani.SetBool("verticalAttack", false);
        _ani.SetBool("horizontalAttack", false);

        _canAttack = true;
    }

    #endregion

    #region Gizmos
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
    #endregion
}