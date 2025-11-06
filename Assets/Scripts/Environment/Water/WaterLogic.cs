using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterLogic : MonoBehaviour
{
    #region Variables

    [Header("References")]
    [SerializeField] private Transform _respawnPoint;

    [Header("RespawnTime")]
    [SerializeField] private float _respawnTime = 0.2f;

    private Transform _playerPos;

    #endregion

    #region Awake
    private void Awake()
    {
        _playerPos = GameObject.FindGameObjectWithTag("Player").transform;
    }
    #endregion

    #region Collisions

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !collision.isTrigger)
        {
            StartCoroutine(WaterDamage());
        }
    }
    #endregion

    #region Coroutine

    private IEnumerator WaterDamage()
    {
        //will trigger sfx of cat + splash
        SFXManager.instance.playSFX("waterSplash");

        //remove one life off player - call take damage function from playerhealth script
        PlayerHealth.instance.TakeDamage(1, new Vector2(_playerPos.position.x, _playerPos.position.y), 0);
        yield return new WaitForSeconds(_respawnTime);
        //will respawn player
        _playerPos.position = _respawnPoint.position;
    }
    #endregion
}
