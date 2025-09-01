using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeAI : MonoBehaviour
{
    #region Variables
    [Header("References")]
    //reference the enemy controller data on this enemy
    private EnemyController _enemyController;

    [Header("Enemy Movement Properties")]
    public Transform pointA;
    public Transform pointB;
    //[SerializeField] private float _hopForceX = 3f;
    [SerializeField] private float _hopForceY = 5f;
    [SerializeField] private float _hopDelay = 0.5f;
    private bool _movingToB = false;

    #endregion

    #region Awake
    private void Awake()
    {
        _enemyController = GetComponent<EnemyController>();
    }
    #endregion

    #region Start
    private void Start()
    {
        StartCoroutine(EnemyBounce());
    }
    #endregion

    #region Bounce Patrol Logic
    private IEnumerator EnemyBounce()
    {
        while (!_enemyController._isDead)
        {
            //skip if in knockback
            if (_enemyController._isKnockedBack)
            {
                yield return null;
                continue;
            }

            //wait until grounded
            yield return new WaitUntil(() => _enemyController.IsGrounded());
            //small pause
            yield return new WaitForSeconds(_hopDelay);

            //calculate direction and face the next direction
            Vector2 targetPos = _movingToB ? pointB.position : pointA.position;
            _enemyController.spriteRenderer.flipX = targetPos.x > transform.position.x;

            //reset horizontal velocity
            _enemyController._rb.velocity = new Vector2(0, _enemyController._rb.velocity.y);

            //calculate hop distance
            float distanceX = targetPos.x - transform.position.x;
            Vector2 hopForce = new Vector2(distanceX, _hopForceY);

            //apply hop force
            _enemyController._rb.AddForce(hopForce, ForceMode2D.Impulse);

            //flip target for next hope
            _movingToB = !_movingToB;
        }

    }
 
    #endregion
}
