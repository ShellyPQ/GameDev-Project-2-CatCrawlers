using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcePlatform : MonoBehaviour
{
    #region Variables
    [Header("Sliding Properties")]
    [SerializeField] private float iceAcceleration = 0;
    [SerializeField] private float iceFriction = 0.1f;
    #endregion

    #region Collisions
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            PlayerController player = collision.collider.GetComponent<PlayerController>();

            if (player != null)
            {
                player.SetIceState(true, iceAcceleration, iceFriction);
            }
        }         
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            PlayerController player = collision.collider.GetComponent<PlayerController>();
            if (player != null)
            {
                player.SetIceState(false, 0f, 0f);
            }
        }
    }

    #endregion
}
