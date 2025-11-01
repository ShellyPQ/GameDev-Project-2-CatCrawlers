using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MudLogic : MonoBehaviour
{

    #region Variables

    [Header("References")]
    [SerializeField] ParticleSystem _slowParticle;
    [SerializeField] ParticleSystem _mudParticle;

    [Header("Script Properties")]
    public bool playerIsSlowed = false;
    [SerializeField] private Rigidbody2D _rb;

    #endregion

    #region Update
    private void Update()
    {
        if (!playerIsSlowed) return;

        //if moving play mud particle
        if (Mathf.Abs(_rb.velocity.x) > 0.05)
        {
            if (!_mudParticle.isPlaying) _mudParticle.Play();
        }
        else
        {
            if (_mudParticle.isPlaying) _mudParticle.Stop();
        }
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //the player is slowed
            playerIsSlowed = true;
            //the player speed is half
            PlayerController.instance.playerSpeed = PlayerController.instance.playerSpeed / 2;
            //trigger particle system (slow)
            _slowParticle.Play();
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //the player is no longer slowed
            playerIsSlowed = false;
            //the players speed goes back to normal
            PlayerController.instance.playerSpeed = PlayerController.instance.playerSpeed * 2;
            //particle system off
            _slowParticle.Stop();
            _mudParticle.Stop();
        }
    }
}