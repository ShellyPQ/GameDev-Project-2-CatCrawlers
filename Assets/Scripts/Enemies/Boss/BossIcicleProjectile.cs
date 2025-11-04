using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIcicleProjectile : MonoBehaviour
{
    #region Variables
    [Header("Projectile Properties")]
    public float speed = 8f;
    public float lifetime = 3f;
    public int damage = 1;
    public Vector2 direction = Vector2.left;

    #endregion

    #region Start
    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
    #endregion

    #region Update
    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }
    #endregion

    #region Collision
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector2 knockDir  = (collision.transform.position - transform.position).normalized;
            collision.GetComponent<PlayerHealth>()?.TakeDamage(damage, knockDir, 5f);
            OnHit();
        }
    }
    #endregion

    #region Method/Functions
    private void OnHit()
    {
        Destroy(gameObject);
    }

    #endregion
}
