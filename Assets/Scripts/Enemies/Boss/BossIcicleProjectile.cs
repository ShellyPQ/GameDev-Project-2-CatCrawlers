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

    private Vector3 _baseScale;
    #endregion

    #region Start
    private void Start()
    {
        //cache the prefab’s natural scale and normalize direction
        _baseScale = transform.localScale;
        direction = direction.sqrMagnitude > 0f ? direction.normalized : Vector2.left;

        //flip only on X to face travel dir without ruining size
        float x = Mathf.Abs(_baseScale.x) * (direction.x < 0f ? -1f : 1f);
        transform.localScale = new Vector3(x, _baseScale.y, _baseScale.z);

        Destroy(gameObject, lifetime);
    }
    #endregion

    #region Update
    private void Update()
    {
        transform.Translate((Vector3)direction * speed * Time.deltaTime, Space.World);
    }
    #endregion

    #region Collision
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector2 knockDir = (collision.transform.position - transform.position).normalized;
            collision.GetComponent<PlayerHealth>()?.TakeDamage(damage, knockDir, 5f);
            Destroy(gameObject);
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
