using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    #region Variables
    [Header("References")]
    [Tooltip("Drag and drop the player object here.")]
    [SerializeField] private Transform _playerTransform;

    [Header("Sprite Rotation Properties")]
    [Tooltip("Sprite rotation lerp time")]
    [SerializeField] private float _spriteYRotationTime = 0.15f;

    private Coroutine _spriteRotationCoroutine;

    private PlayerController _playerController;

    private bool _isFacingRight;
    #endregion

    #region Awake
    private void Awake()
    {
        //Assign the player object holding the player controller script 
        _playerController = _playerTransform.gameObject.GetComponent<PlayerController>();

        //Check in what direction the player is facing 
        _isFacingRight = _playerController._isFacingRight;
    }
    #endregion

    #region Update
    private void FixedUpdate()
    {
        //Have this object (camera follow) follow the player's positions
        //The camera will follow this object instead of the player
        transform.position = _playerTransform.position;
    }
    #endregion

    #region Method/Functions

    //Call this coroutine function when the player is moving in the oposite direction that the player sprite is facing
    public void CallSpriteFlip()
    {
        _spriteRotationCoroutine = StartCoroutine(FlipYLerp());
    }

    //Flip the object based on what direction the player is moving. Lerp this rotation to smooth out the camera movement
    private IEnumerator FlipYLerp()
    {
        float startRotation = transform.localEulerAngles.y;
        float endRotationAmount = CheckEndRotation();
        float yRot = 0f;

        float elapsedTime = 0f;
        while (elapsedTime < _spriteYRotationTime)
        {
            elapsedTime += Time.deltaTime;

            //lerp the y rotation
            yRot = Mathf.Lerp(startRotation,endRotationAmount, (elapsedTime / _spriteYRotationTime));
            transform.rotation = Quaternion.Euler(0f, yRot, 0f);

           yield return null;
        }
    }

    //Check in what direction the player is moving
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
