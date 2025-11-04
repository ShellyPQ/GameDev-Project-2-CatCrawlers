using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CatnipFrenzyBuff : MonoBehaviour
{
    #region Variables

    [Header("Player References")]
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Animator _playerAni;
    [SerializeField] private CapsuleCollider2D _playerCollider;

    [Header("VisualFX")]
    [Tooltip("Panel that changes colours when buff is active.")]
    [SerializeField] private ScriptableRendererFeature _frenzyShaderFeature;
    [SerializeField] private Material _frenzyShader;

    [Header("Frenzy Collider Size")]
    [SerializeField] private Vector2 frenzyColliderSize = new Vector2(2f, 5f);

    [Header("Frenzy BGM/SFX")]
    [SerializeField] private AudioClip _frenzyMusic;
    [SerializeField] private AudioClip _normalMusic;
    [SerializeField] private float _fadeMusicTime = 0.3f;
    [SerializeField] private AudioSource _frenzyPurrSFX;

    private float _originalSpeed;
    private Vector2 _originalColliderSize;

    #endregion

    #region Awake
    private void Awake()
    {
        _playerCollider = GetComponent<CapsuleCollider2D>();
        if (_playerCollider != null)
        {
            _originalColliderSize = _playerCollider.size;
        }
    }
    #endregion

    #region Start
    private void Start()
    {
        _frenzyShaderFeature.SetActive(false);
    }
    #endregion

    #region Method/Functions

    public void StartFrenzy(float duration, float frenzySpeed)
    {
        StartCoroutine(FrenzyRoutine(duration, frenzySpeed));
    }

    #region Coroutine

    private IEnumerator FrenzyRoutine(float duration, float frenzySpeed)
    {
        MusicManager.instance.PlayMusic(_frenzyMusic, _fadeMusicTime);

        //start purr sfx
        _frenzyPurrSFX.clip = SFXManager.instance.GetClip("catFrenzyPurr");
        _frenzyPurrSFX.Play();

        //save original speed values and set new speed
        _originalSpeed = _playerController.playerSpeed;
        _playerController.playerSpeed = frenzySpeed;

        _playerAni.SetBool("isFrenzied", true);
        _playerHealth.SetInvulnerable(true);
        CatnipDamageSystem.EnableFrenzyMode();

        //turn on overlay shader
        _frenzyShaderFeature.SetActive(true);

        yield return null;
        ShrinkCollider();

        yield return new WaitForSeconds(duration);

        //stop purring sfx
        _frenzyPurrSFX.Stop();

        //reset everything after buff runs out
        _playerController.playerSpeed = _originalSpeed;
        _playerAni.SetBool("isFrenzied", false);

        _playerHealth.SetInvulnerable(false);
        CatnipDamageSystem.DisableFrenzyMode();

        _frenzyShaderFeature.SetActive(false);

        yield return null;
        ResetCollider();

        MusicManager.instance.PlayMusic(_normalMusic, _fadeMusicTime);
    }

    #endregion

    private void ShrinkCollider()
    {
        if( _playerCollider == null ) return;

        _playerCollider.size = frenzyColliderSize;
    }

    private void ResetCollider()
    {
        if (_playerCollider == null ) return;

        _playerCollider.size = _originalColliderSize;
    }
    #endregion


}
