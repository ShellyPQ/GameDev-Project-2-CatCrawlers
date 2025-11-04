using System.Collections;
using UnityEngine;

public class GolemBossAI : MonoBehaviour
{
    #region Variables

    [Header("Boss Settings")]
    public bool bossActive = false;
    public float idleTime = 2f;

    [Header("Reference")]
    public Transform firePoint;
    public GameObject icicleProjectile;

    private Animator _ani;
    private EnemyController _enemy;

    private bool isAttacking = false;

    #endregion

    #region Awake
    private void Awake()
    {
        _ani = GetComponent<Animator>();
        _enemy = GetComponent<EnemyController>();

        //boss hp override
        _enemy.maxHealth = 3;
        //_enemy.canDamagePlayer = false;
        //_enemy.isBoss = true;
    }
    #endregion

    #region Method/Functions
    public void ActivateBoss()
    {
        if (bossActive) return;
        bossActive = true;
        StartCoroutine(BossLoop());
    }

    private IEnumerator BossLoop()
    {
        yield return new WaitForSeconds(1f);

        while (!_enemy._isDead)
        {
            //slam attack
            yield return new WaitForSeconds(idleTime);
            TriggerSlam();
            yield return new WaitUntil(() => !isAttacking);

            //range attack
            yield return new WaitForSeconds(idleTime);
            TriggerRangeAttack();
            yield return new WaitUntil(() => !isAttacking);
        }
    }

    private void TriggerSlam()
    {
        isAttacking = true;
        _ani.SetTrigger("groundAttack");
    }

    private void TriggerRangeAttack()
    {
        isAttacking = true;
        _ani.SetTrigger("rangeAttack");
    }

    //animation event
    public void DoSlamEffect()
    {
        //BossSpikeSpawner.instance.SpawnSpikes();
        //screenshake effect
    }

    //animation event
    public void FireIcicle()
    {
        GameObject proj = Instantiate(icicleProjectile, firePoint.position, Quaternion.identity);

        BossIcicleProjectile icicle = proj.GetComponent<BossIcicleProjectile>();

        //straight shot toward player but only horizontal
        Vector2 dir = PlayerHealth.instance.transform.position - firePoint.position; ;
        icicle.direction = new Vector2(Mathf.Sin(dir.x), 0);
    }

    //animation event 
    public void AttackFinished()
    {
        isAttacking = false;
    }
    #endregion
}