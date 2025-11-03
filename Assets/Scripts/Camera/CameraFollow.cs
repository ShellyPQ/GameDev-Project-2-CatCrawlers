using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    #region Variables
    [Header("References")]
    [Tooltip("Drag and drop the player object here.")]
    [SerializeField] private Transform _playerTransform;

    [Header("Sprite Rotation Properties")]
    [Tooltip("Sprite rotation lerp time, higher = slower/smoother")]
    [SerializeField] private float _spriteYRotationTime = 0.25f;

    private Coroutine _spriteRotationCoroutine;
    private PlayerController _playerController;
    private bool _isFacingRight;
    #endregion

    #region Awake
    private void Awake()
    {
        //assign the player object holding the player controller script
        _playerController = _playerTransform.gameObject.GetComponent<PlayerController>();

        //check in what direction the player is facing
        _isFacingRight = _playerController._isFacingRight;
    }
    #endregion

    #region LateUpdate
    private void LateUpdate()
    {
        //have this object (camera follow) follow the player's positions 
        //the camera will follow this object instead of the player
        transform.position = _playerTransform.position;
    }
    #endregion

    #region Method/Functions

    //call this coroutine function when the player is moving in the oposite direction that the player sprite is facing
    public void CallSpriteFlip()
    {
        if (_spriteRotationCoroutine != null)
        {
            StopCoroutine(_spriteRotationCoroutine);
        }
        _spriteRotationCoroutine = StartCoroutine(FlipYLerp());
    }

    //flip the object based on what direction the player is moving. Lerp this rotation to smooth out the camera movement
    private IEnumerator FlipYLerp()
    {
        float startRotation = transform.localEulerAngles.y;
        float endRotation = CheckEndRotation();

        float elapsedTime = 0f;

        while (elapsedTime < _spriteYRotationTime)
        {
            elapsedTime += Time.deltaTime;

            //smoothstep for easing 
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / _spriteYRotationTime);

            float yRot = Mathf.LerpAngle(startRotation, endRotation, t);
            transform.rotation = Quaternion.Euler(0f, yRot, 0f);

            yield return null;
        }
    }

    //check in what direction the player is moving
    private float CheckEndRotation()
    {
        _isFacingRight = !_isFacingRight;

        if (_isFacingRight)
        {
            return 0f;
        }
        else
        {
            return 180f;
        }
    }
    #endregion
}
