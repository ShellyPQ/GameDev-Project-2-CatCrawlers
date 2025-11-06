using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceSpikeDamage : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector2 dir = (collision.transform.position - transform.position).normalized;
            collision.GetComponent<PlayerHealth>()?.TakeDamage(damage, dir, 5f);
        }
    }

    private void OnEnable()
    {
        Physics2D.SyncTransforms();
    }
}
