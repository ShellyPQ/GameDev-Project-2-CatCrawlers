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

    [Header("Slam Spikes")]
    public Animator spikeSet1;
    public Animator spikeSet2;
    private int slamIndex = 0;

    [Header("Boss Music")]
    public AudioClip bossMusic;
    public AudioClip normalMusic;
    public float fadeTime = 0.3f;

    private Animator _ani;
    private EnemyController _enemy;
    private bool isAttacking = false;

    private readonly int isCat = Animator.StringToHash("isCat");

    #endregion

    #region Awake
    private void Awake()
    {
        _ani = GetComponent<Animator>();
        _enemy = GetComponent<EnemyController>();

        //boss hp override
        _enemy.maxHealth = 3;
        //_enemy.canDamagePlayer = false;
        _enemy.isBoss = true;
    }
    #endregion

    #region Method/Functions
    public void ActivateBoss()
    {
        if (bossActive) return;
        bossActive = true;

        if (MusicManager.instance != null && bossMusic != null)
            MusicManager.instance.PlayMusic(bossMusic, fadeTime);
        else
            Debug.Log("Boss music or music manager is mising");

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
        slamIndex++;

        if (slamIndex % 2 == 1)
            spikeSet1.SetTrigger("iceUp");
        else
            spikeSet2.SetTrigger("iceUp");
    }

    //animation event
    public void FireIcicle()
    {
        //decide horizontal direction toward the player
        Vector2 toPlayer = PlayerHealth.instance.transform.position - firePoint.position;
        float dirX = Mathf.Sign(toPlayer.x);

        //instantiate with neutral rotation
        GameObject proj = Instantiate(icicleProjectile, firePoint.position, Quaternion.identity);

        proj.transform.localScale = icicleProjectile.transform.localScale;

        var icicle = proj.GetComponent<BossIcicleProjectile>();
        if (icicle != null)
            icicle.direction = new Vector2(dirX, 0f);
    }

    //animation event 
    public void AttackFinished()
    {
        isAttacking = false;
    }
    #endregion
}