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
    [Tooltip("How much the camera lifts during a jump.")]
    [SerializeField] private float _maxJumpOffset = 0.35f;
    [Tooltip("How much the camera lowers during a fall.")]
    [SerializeField] private float _fallYOffset = -0.45f;
    [Tooltip("Small upward pan when grounded. Positive values makes the camera higher.")]
    [SerializeField] private float _groundYOffset = 0.2f;
    [Tooltip("How smoothly thhe camera moves between offsets.")]
    [SerializeField] private float _panSmoothSpeed = 3.5f;

    private CinemachineFramingTransposer _framingTransposer;
    private Vector3 _defaultOffset;

    private float _currentYOffset;
    private float _targetYOffset;

    #endregion

    #region Awake
    private void Awake()
    {
        _framingTransposer = _virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        //lower the camera's default y offset so the ground is always more visible
        _defaultOffset = Vector3.zero;
        _framingTransposer.m_TrackedObjectOffset = _defaultOffset;
    }
    #endregion

    #region Method/Functions

    //smoothly adjust the camera framing based on the players vertical velocity
    public void smoothFollowY(float yVelocity, bool isGrounded)
    {
        //default yoffset set toward the ground
        _targetYOffset = 0f;

        if (!isGrounded)
        {
            if (yVelocity > 0.1f)
            {
                _framingTransposer.m_DeadZoneHeight = 0.55f;
                _targetYOffset = _maxJumpOffset * 0.5f;
            }
            else if (yVelocity < -0.1f)
            {
                _framingTransposer.m_DeadZoneHeight = 0.25f;
                _targetYOffset = _fallYOffset;
            }
        }      
        else
        {
            //grounded or moving horizontally, reset dead zone height
            _framingTransposer.m_DeadZoneHeight = 0.35f;
            _targetYOffset = _groundYOffset;
        }

        //cap the upward offset toward target
        _targetYOffset = Mathf.Clamp(_targetYOffset, _fallYOffset, _maxJumpOffset);

        //smoothly move offset toward target
        _currentYOffset = Mathf.Lerp(_currentYOffset, _targetYOffset, Time.deltaTime * _panSmoothSpeed);

        _framingTransposer.m_TrackedObjectOffset = new Vector3(0, _currentYOffset, 0);
    }

    #endregion
}
