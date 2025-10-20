using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeDummy : MonoBehaviour, IDamageable
{
    #region Variables
    [Header("Dummy Properties")]
    [SerializeField] private int _maxHealth = 1;
    [SerializeField] private float _shakeDuration = 0.2f;
    [SerializeField] private float _shakeStrength = 0.1f;

    private int _currentHealth;
    private bool _isDead = false;

    [Header("Reference")]
    [SerializeField] private Animator triggerAni;
    private Collider2D _hitbox;

    [Header("References")]
    //Reference to player to access attack script
    private GameObject _player;
    #endregion

    #region Awake
    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }
    #endregion

    #region Start
    private void Start()
    {
        //disable the players melee and range attack at the start of the tutorial
        _player.gameObject.GetComponent<MeleeAttack>().enabled = false;
        
        _currentHealth = _maxHealth;
        _hitbox = GetComponent<Collider2D>();
    }
    #endregion

    #region Method/Functions
    public void TakeDamage(int dmg, Vector2 hitDirection)
    {
        //have dummy shake when hit (only one hit will eliminate dummy - coroutine to allow this to fire then fire the Die function)

        if (_isDead)
        {
            return;
        }

        _currentHealth -= dmg;

        //visual damage feedback
        StartCoroutine(ShakeEffect());

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    //make the dummy shake when the player hits it
    private IEnumerator ShakeEffect()
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < _shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * _shakeStrength;
            float y = Random.Range(-1f, 1f) * _shakeStrength;

            transform.localPosition = originalPos + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }

    //disable dummy collider and trigger gate animation
    public void Die()
    {
        _isDead = true;

        //trigger particle effect
        GetComponent<DummyParticle>()?.PlayParticleEffect();

        //trigger gate animation
        if (triggerAni != null)
        {
            triggerAni.SetBool("canProceed", true);
            // Play gate open SFX
            SFXManager.instance.playSFX("doorUnlock");
        }
        else 
        {
            Debug.Log("Gate animator not assigned in the inspector!");
        }

        //enable full yarn ammo after tutorial dummy is hit
        if (_player != null)
        {
            var ranged = _player.GetComponent<RangedAttack>();
            ranged?.UnlockMaxAmmo();
            _player.GetComponent<MeleeAttack>().enabled = true;
            _player.GetComponent<RangedAttack>().enabled = true;
        }

        //disable collider so player can walkthrough
        if (_hitbox != null)
        {
            _hitbox.enabled = false;
        }
    }
    #endregion

    #region Debug Gizmos

    private void OnDrawGizmos()
    {
        if (gameObject.GetComponent<BoxCollider2D>() != null)
        {
            Gizmos.color = Color.green;
            
            BoxCollider2D boxCollider = gameObject.GetComponent<BoxCollider2D>();

            // Get the center and size of the collider in world space
            Vector2 worldCenter = (Vector2)transform.position + boxCollider.offset;
            Vector2 worldSize = boxCollider.size;

            //Draw a wire cube to represent the box collider of the object            
            Gizmos.DrawWireCube(worldCenter, worldSize);
        }
    }
    #endregion
}
