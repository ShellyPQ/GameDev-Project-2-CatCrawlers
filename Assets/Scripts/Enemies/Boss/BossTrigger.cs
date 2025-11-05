using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    #region Variables

    public GolemBossAI boss;
    public Animator iceWallAni1;
    public Animator iceWallAni2;

    #endregion

    #region Collision
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            boss.ActivateBoss();

            //change song to boss battle

            
            //trigger ice wall animation
            if (iceWallAni1 != null && iceWallAni2 != null)
            {
                iceWallAni1.SetBool("triggerIceWall", true);
                iceWallAni2.SetBool("triggerIceWall", true);
            }

            PlayerHealth.instance.inBossFight = true;
            gameObject.SetActive(false);
        }
    }

    #endregion
}
