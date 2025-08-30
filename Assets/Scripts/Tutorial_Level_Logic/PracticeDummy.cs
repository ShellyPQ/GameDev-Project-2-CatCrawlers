using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PracticeDummy : MonoBehaviour
{
    #region Variables
    [Header("References")]
    //Reference to player to access attack script
    private GameObject _player;

    [Header("Properties")]
    [Tooltip("Melee Training Dummy Trigger")]
    [SerializeField] private int _DummyHealth = 1;
    private bool _isDead = false;
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
        //disable the players attack at the start of the tutorial
        _player.gameObject.GetComponent<MeleeAttack>().enabled = false;
    }
    #endregion

    #region Method/Functions
    public void TakeDamage(int dmg, Vector2 hitDirection)
    {
        if (_isDead)
        {
            return;
        }

        _DummyHealth -= dmg;

        if (_DummyHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        _isDead = true;

        //trigger fall over animation

        //trigger gate animation

        //have dummy stay in background like the cat
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
