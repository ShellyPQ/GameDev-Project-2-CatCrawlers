using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YarnProjectile : MonoBehaviour
{
    #region Variables
    [Header("Projectile Properties")]
    public float projectileSpeed = 10f;
    public float maxDistance = 5f;
    public float stunDuration = 2f;

    [Header("Visual Properties")]
    [SerializeField] private ParticleSystem wrapEffectPrefab;

    private Vector3 _startPos;
    private Vector3 _dir;

    //reference to player attack script for ammo
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
    public void Initialize(Vector3 direction, float speed, float range, float stun, RangedAttack rangedAttack)
    {
        _dir = direction.normalized;
        _startPos = transform.position;
        projectileSpeed = speed;
        maxDistance = range;
        stunDuration = stun;
        _rangedAttack = rangedAttack;
    }
    #endregion

    #region Trigger Logic
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //enemy hit
        if (collision.CompareTag("Enemy"))
        {
            EnemyController enemy = collision.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.Stun(stunDuration);

                //spawn wrap particle
                if (wrapEffectPrefab != null)
                {
                    ParticleSystem wrap = Instantiate(wrapEffectPrefab, enemy.transform.position, Quaternion.identity);
                    wrap.transform.parent = enemy.transform;
                    wrap.Play();
                    Destroy(wrap.gameObject, stunDuration);
                }

                //consume ammo after successful enemy hit
                _rangedAttack.ConsumeAmmo();
            }
            Destroy(gameObject);
        }
        //dummy or environment hit (puzzle hits)
        else if (collision.CompareTag("PracticeDummy") || collision.CompareTag("Environment"))
        {
            Destroy(gameObject);
        }
    }
    #endregion
}
