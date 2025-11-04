using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    #region Variables

    public GolemBossAI boss;

    #endregion

    #region Collision
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            boss.ActivateBoss();
            gameObject.SetActive(false);
        }
    }

    #endregion
}
