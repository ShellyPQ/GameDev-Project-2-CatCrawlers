using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YarnProjectile : MonoBehaviour
{
    #region Variables
    [Header("Projectile Properties")]
    public float projectileSpeed = 10f;
    public float maxDistance = 5f;

    private Vector3 _startPos;
    private Vector3 _dir;
    private RangedAttack _rangedAttack;
    #endregion

    #region Update
    private void Update()
    {
        transform.position += _dir * projectileSpeed * Time.deltaTime;

        if (Vector3.Distance(_startPos, transform.position) >= maxDistance)
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region Initialization
    public void Initialize(Vector3 direction, float speed, float range, RangedAttack rangedAttack)
    {
        _dir = direction.normalized;
        _startPos = transform.position;
        projectileSpeed = speed;
        maxDistance = range;
        _rangedAttack = rangedAttack;
    }
    #endregion

    #region Trigger Logic
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //enemy hit (or ranged dummy)
        if (collision.CompareTag("Enemy"))
        {
            EnemyController enemy = collision.GetComponent<EnemyController>();
            enemy?.Stun(2f);

            //play hit sfx
            SFXManager.instance.playSFX("yarnStun");

            //_rangedAttack?.ConsumeAmmo();
            Destroy(gameObject);
        }
        else if (collision.CompareTag("RangedDummy"))
        {
            EnemyStunEffect stunEffect = collision.GetComponent<EnemyStunEffect>();
            if (stunEffect != null)
            {
                stunEffect.Stun();
            }

            RangedDummy dummy = collision.GetComponent<RangedDummy>();
            if (dummy != null)
            {
                dummy.Stun();
            }

            //play hit sfx
            SFXManager.instance.playSFX("yarnStun");

            //_rangedAttack?.ConsumeAmmo();
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Environment"))
        {
            Destroy(gameObject);
        }
    }
    #endregion
}
