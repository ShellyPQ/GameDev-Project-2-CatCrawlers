using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RangedDummy : MonoBehaviour
{
    [Header("Dummy Properties")]
    [SerializeField] private Animator _triggerAni;
    private EnemyStunEffect _stunEffect;
    private Collider2D _hitbox;
    private GameObject _player;
    private bool _isStunned = false;

    [Header("References")]
    [SerializeField] private GameObject _ammoHUD;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Start()
    {
        _hitbox = GetComponent<Collider2D>();
        _stunEffect = GetComponent<EnemyStunEffect>();

        // Ensure ranged attack is disabled until tutorial trigger
        _player.GetComponent<RangedAttack>().enabled = false;

        //disable ammo HUD at start
        if (_ammoHUD != null)
        {
            _ammoHUD.SetActive(false);
        }
    }

    // Only called by YarnProjectile
    public void Stun()
    {
        if (_isStunned) return;

        _isStunned = true;

        // Trigger subtle shake
        _stunEffect?.Stun();

        // Disable collider so player can walk through
        if (_hitbox != null)
            _hitbox.enabled = false;

        // Trigger door animation
        _triggerAni?.SetBool("canProceed", true);

        Debug.Log("Ranged Tutorial Complete");
    }
}
