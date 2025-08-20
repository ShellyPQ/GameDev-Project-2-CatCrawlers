using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    [SerializeField] private CinemachineVirtualCamera[] _allVirtualCameras;

    [Header("Jump/Fall Pan Properties")]
    [Tooltip("Damping change when falling")]
    [SerializeField] private float _fallPanAmount = 0.25f;
    [Tooltip("Time to lerp")]
    [SerializeField] private float _fallYPanTime = 0.35f;
    [Tooltip("How far the camera pans down")]
    [SerializeField] private float _fallYOffset = -2f;
    [Tooltip("Camera pan up on jump")]
    [SerializeField] private float _jumpYOffset = 1f;
    [Tooltip("Velocity threshold")]
    public float fallSpeedYDampingChangeThreshold = -15f;

    public bool isLerpingYDamping { get; private set; }
    public bool LerpedFromPlayerFalling { get; set; }

    private Coroutine _lerpCoroutine;

    private CinemachineVirtualCamera _currentCamera;
    private CinemachineFramingTransposer _framingTransposer;

    private float _normYPanDamping;
    private Vector3 _normTrackedOffset;

    #region Awake
    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
        }

        for (int i = 0; i < _allVirtualCameras.Length; i++)
        {
            if (_allVirtualCameras[i].enabled)
            {
                //set the current active camera
                _currentCamera = _allVirtualCameras[i];

                //set the framing transposer
                _framingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            }
        }

        //Store the normal values
        _normYPanDamping = _framingTransposer.m_YDamping;
        _normTrackedOffset = _framingTransposer.m_TrackedObjectOffset;
    }
    #endregion

    #region Method/Functions

    #region Lerp Y Damping
    //call this when the player starts falling
    public void LerpYPanAndOffset(bool isJumping, bool isFalling)
    {
        if (_lerpCoroutine != null)
        {
            StopCoroutine(_lerpCoroutine);
        }
        _lerpCoroutine = StartCoroutine(LerpPanCoroutine(isJumping, isFalling));
    }

    private IEnumerator LerpPanCoroutine(bool isJumping, bool isFalling)
    {
        isLerpingYDamping = true;

        //starting values
        float startDamping = _framingTransposer.m_YDamping;
        Vector3 startOffset = _framingTransposer.m_TrackedObjectOffset;

        //target values
        float targetDamping = _normYPanDamping;
        Vector3 targetOffset = _normTrackedOffset;

        float lerpDuration = _fallYPanTime;

        if (isFalling)
        {
            targetDamping = _fallPanAmount;
            targetOffset = _normTrackedOffset + new Vector3(0, _fallYOffset, 0);
            lerpDuration = _fallYPanTime;
            LerpedFromPlayerFalling = true;
        }
        else if (isJumping)
        {
            targetOffset = _normTrackedOffset + new Vector3(0, _jumpYOffset, 0);
            lerpDuration = 0.1f;
        }

        float elapsed = 0f;
        while (elapsed < lerpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / lerpDuration;

            _framingTransposer.m_YDamping = Mathf.Lerp(startDamping, targetDamping, t);
            _framingTransposer.m_TrackedObjectOffset = Vector3.Lerp(startOffset, targetOffset, t);

            yield return null;
        }
        isLerpingYDamping = false;
    }
    #endregion

    #endregion
}
