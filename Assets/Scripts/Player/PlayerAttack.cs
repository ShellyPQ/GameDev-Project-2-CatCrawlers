using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    #region Variables
    [Header("Attack Properties")]
    public float attackRange = 1f;
    public int damage = 1;
    public Transform attackPoint;
    public LayerMask enemyMask;
    #endregion

    #region Update
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }
    }
    #endregion

    #region Method/Functions
    private void Attack()
    {
        //Detect enemies in a small circle in front of the player
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange,enemyMask);

        foreach (Collider2D enemy in hitEnemies)
        {
            //calculate direction from player to enemy
            Vector2 hitDir = (enemy.transform.position - transform.position).normalized;

            enemy.GetComponent<EnemyController>()?.TakeDamage(damage, hitDir);
        }
    }
    #region Debug Gizmos
    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
    #endregion

    #endregion
}
