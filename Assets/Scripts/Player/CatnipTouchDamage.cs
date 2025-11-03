using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatnipTouchDamage : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!CatnipDamageSystem.frenzyActive) return;

        EnemyController enemy = collision.GetComponent<EnemyController>();

        if (enemy != null && !enemy._isDead)
        {
            //pass a direction so enemy still reacts to damage
            Vector2 hitDir = (enemy.transform.position - transform.position).normalized;
            enemy.TakeDamage(enemy.maxHealth, hitDir);
        }
    }
}
