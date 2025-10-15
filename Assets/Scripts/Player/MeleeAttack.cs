using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    #region Variables
    [Header("Attack Properties")]
    [SerializeField] private float _attackRange = 1f;
    [SerializeField] private int _damage = 1;
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private LayerMask _enemyMask;
    [SerializeField] private Vector3 _attackPointOffset = new Vector3(1f, 0f, 0f);

    [Header("Visual Effects Properties")]
    [Tooltip("Swipe Particle Effect")]
    [SerializeField] private ParticleSystem _swipeLeftPrefab;
    [SerializeField] private ParticleSystem _swipeRightPrefab;
    [SerializeField] private int _particlePoolSize = 5;

    //arrays for left and right direction particles
    private ParticleSystem[] _leftPool;
    private ParticleSystem[] _rightPool;
    //index to track next particle to play
    private int _leftPoolIndex = 0;
    private int _rightPoolIndex = 0;

    //reference to the attached player controler
    private PlayerController _playerController;
    private Animator _ani;
    #endregion

    #region Awake
    private void Awake()
    {
        //assign the player controller on awake
        _playerController = GetComponent<PlayerController>();
        //assign the player animator on awake
        _ani = GetComponent<Animator>();

        //set up particle pool/array
        InitializeParticlePools();
    }
    #endregion

    #region Update
    private void Update()
    {
        //if left mouse click an if mouse click in the direction the player is facing
        if (InputManager.instance.attackMelee)
        {
            Attack();

            //animation triggers
            _ani.SetBool("isAttacking", true);
            _ani.SetInteger("attackType", 1);
            _ani.SetTrigger("attackTrigger");
        }
    }
    #endregion

    #region Melee Attack Method
    public void Attack()
    {
        //update attack point position
        UpdateAttackPoint();

        //play particle
        SwipeParticle();

        //detect enemies in attack range
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(_attackPoint.position, _attackRange, _enemyMask);

        foreach (Collider2D enemy in hitEnemies)
        {
            Vector2 hitDir = (enemy.transform.position - transform.position).normalized;

            //apply damage to enemy
            enemy.GetComponent<EnemyController>()?.TakeDamage(_damage, hitDir);
            enemy.GetComponent<MeleeDummy>()?.TakeDamage(_damage, hitDir);
        }
    }
 
    private void UpdateAttackPoint()
    {
        //move attack point in front of player based on what dir the player is facing
        if (_playerController != null && _attackPoint != null)
        {
            float dir = _playerController._isFacingRight ? 1f : -1f;
            _attackPoint.position = transform.position + new Vector3(_attackPointOffset.x * dir, _attackPointOffset.y, _attackPointOffset.z);
        }
    }
    #endregion

    #region Attack Particle Effect Method
    private void SwipeParticle()
    {
        if (_playerController == null) return;

        //select particle from pool based on what direction the player is facing
        ParticleSystem particleSystem;
        if (_playerController._isFacingRight)
        {
            particleSystem = _rightPool[_rightPoolIndex];
            _rightPoolIndex = (_rightPoolIndex + 1) % _rightPool.Length;
        }
        else
        {
            particleSystem = _leftPool[_leftPoolIndex];
            _leftPoolIndex = (_leftPoolIndex + 1) % _leftPool.Length;
        }

        //move and reset particle
        particleSystem.transform.position = _attackPoint.position;
        particleSystem.transform.rotation = Quaternion.Euler(0f, 0f, 45f);
        particleSystem.transform.localScale = Vector3.one;

        particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        particleSystem.Play();

        //play melee hit sound
        SFXManager.instance.playSFX("meleeSwipe");
    }

    private void InitializeParticlePools()
    {
        //create pools for left and right swipe particles
        _leftPool = new ParticleSystem[_particlePoolSize];
        _rightPool = new ParticleSystem[_particlePoolSize];

        for (int i = 0; i < _particlePoolSize; i++)
        {
            ParticleSystem left = Instantiate(_swipeLeftPrefab, _attackPoint.position, Quaternion.identity, transform);
            left.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            _leftPool[i] = left;

            ParticleSystem right = Instantiate(_swipeRightPrefab, _attackPoint.position, Quaternion.identity, transform);
            right.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            _rightPool[i] = right;
        }
    }
    #endregion

    #region Debug Gizmos
    private void OnDrawGizmosSelected()
    {
        if (_attackPoint != null)
        {
            Gizmos.color = Color.green;
            //show attack range during play testing (scene view)
            Gizmos.DrawWireSphere(_attackPoint.position, _attackRange);
        }
    }
    #endregion
}

