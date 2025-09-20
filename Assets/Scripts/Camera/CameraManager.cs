using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    #region Variables

    [Header("Cinemachine Settings")]
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] private float _jumpyOffset = 0.5f;
    [SerializeField] private float _fallYOffset = -0.5f;
    [SerializeField] private float _panSmoothSpeed = 5f;

    private CinemachineFramingTransposer _framingTransposer;
    private Vector3 _defaultOffset;

    #endregion

    #region Awake
    private void Awake()
    {
        _framingTransposer = _virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

        if (instance == null)
        {
            instance = this;
        }

        
        _defaultOffset = _framingTransposer.m_TrackedObjectOffset;
    }
    #endregion

    #region Method/Functions

    public void smoothFollowY(float yVelocity)
    {
        //get target offset
        Vector3 targetOffset = _defaultOffset;

        if (yVelocity > 0.1f)
        {
            targetOffset.y += _jumpyOffset;
        }
        else if (yVelocity < -0.1f)
        {
            targetOffset.y += _fallYOffset;
        }

        //smoothly interpolate towards target
        _framingTransposer.m_TrackedObjectOffset = Vector3.Lerp(_framingTransposer.m_TrackedObjectOffset, targetOffset, Time.deltaTime * _panSmoothSpeed);
    }
    
    #endregion
}
