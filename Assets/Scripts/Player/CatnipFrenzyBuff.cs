using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatnipFrenzyBuff : MonoBehaviour
{
    #region Variables

    [Header("Player References")]
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Animator _playerAni;

    [Header("VisualFX")]
    [Tooltip("Panel that changes colours when buff is active.")]
    //[SerializeField] private GameObject _catnipBuffOverlayPanel;

    private float _originalSpeed;
    #endregion

    #region Method/Functions

    public void StartFrenzy(float duration, float frenzySpeed)
    {
        StartCoroutine(FrenzyRoutine(duration, frenzySpeed));
    }

    #endregion

    #region Coroutine

    private IEnumerator FrenzyRoutine(float duration, float frenzySpeed)
    {
        //save original speed values
        _originalSpeed = _playerController.playerSpeed;

        //set frenzy settings
        _playerController.playerSpeed = frenzySpeed;
        //_playerHealth.invulnerable = true;

        _playerAni.SetBool("isFrenzied", true);

        //turn on overlay shader
        //if (_catnipBuffOverlayPanel != null)
        //    _catnipBuffOverlayPanel.SetActive(true);

        //destroy/1 hit enemies
        CatnipDamageSystem.EnableFrenzyMode();

        yield return new WaitForSeconds(duration);

        //reset everything after buff runs out
        _playerController.playerSpeed = _originalSpeed;
        //_playerHealth.invulnerable = false;
        _playerAni.SetBool("isFrenzied", false);

        //if (_catnipBuffOverlayPanel != null)
        //    _catnipBuffOverlayPanel.SetActive(false);

        CatnipDamageSystem.DisableFrenzyMode();
    }


    #endregion
}
