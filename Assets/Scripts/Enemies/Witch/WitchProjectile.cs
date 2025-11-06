using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitchProjectile : MonoBehaviour
{
    #region Variables

    [Header("Projectile properties")]
    public float projLifetime = 2f;
    public int projDamage = 1;

    #endregion

    #region Start
    private void Start()
    {
        Destroy(gameObject, projLifetime);    
    }

    #endregion

    #region Trigger Check
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector2 knockDir = (collision.transform.position - transform.position).normalized;
            collision.GetComponent<PlayerHealth>()?.TakeDamage(projDamage, knockDir, 5f);
            Destroy(gameObject);
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("ground"))
        {
            Destroy(gameObject);
        }
    }

    #endregion
}
