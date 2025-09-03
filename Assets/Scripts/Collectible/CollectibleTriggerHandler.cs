using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleTriggerHandler : MonoBehaviour
{
    #region Variables
    //reference to our collectable manager
    private CollectibleManager _collectableManager;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;

    private bool _isRespawning = false;
    #endregion

    #region Awake
    private void Awake()
    {
        _collectableManager = GetComponent<CollectibleManager>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
    }
    #endregion

    #region Method/Functions
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _collectableManager.Collect(collision.gameObject);

            SFXManager.instance.playSFX("pawToken");

            //disable visuals & collider
            _spriteRenderer.enabled = false;
            _collider.enabled = false;

            //start respawn check if not already running
            if (!_isRespawning)
            {
                CollectibleYarnBitSO yarnData = _collectableManager.GetCollectibleSO() as CollectibleYarnBitSO;
                if (yarnData != null && yarnData.respawnTime > 0)
                {
                    StartCoroutine(RespawnCheck(yarnData.respawnTime));
                }
            }
        }
    }

    private IEnumerator RespawnCheck(float delay)
    {
        _isRespawning = true;

        while (true)
        {
            yield return new WaitForSeconds(delay);

            RangedAttack playerRanged = GameObject.FindGameObjectWithTag("Player")?.GetComponent<RangedAttack>();
            if (playerRanged != null && playerRanged.GetAmmo() < playerRanged.maxAmmo)
            {
                //respawn collectible
                _spriteRenderer.enabled = true;
                _collider.enabled = true;
                _isRespawning = false;
                yield break; 
            }
        }       
    }
    #endregion
}
