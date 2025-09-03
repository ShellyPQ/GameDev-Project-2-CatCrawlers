using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables
    public static PlayerController instance;

    [Header("Movement Properties")]
    [Tooltip("Player movements speed")]
    [SerializeField] private float _playerSpeed = 5f;

    [Header("Jump Properties")]
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private float _jumpTime = 0.5f;

    [Header("Ground Check Properties")]
    [SerializeField] private float _extraHeight = 0.35f;
    [SerializeField] private LayerMask _groundMask;

    [Header("Camera Properties")]
    [SerializeField] private GameObject _cameraFollowObject;


    //Variable that will hold the players sprite renderer
    private SpriteRenderer _renderer;
    //Variable that will hold the players rigidbody
    private Rigidbody2D _rb;
    //Variable that will hold the players collider
    private Collider2D _playerCollider;
    //Variable that will hold the float movement input
    private float _moveInput;
    //bool to check in what direction the player is moving
    public bool _isFacingRight = true;

    //Bool to check if the player is jumping
    private bool _isJumping;
    //Bool to check if the player is falling
    private bool _isFalling;
    //Bool check to see if the player can jump again
    private float _jumpTimeCounter;

    private RaycastHit2D _groundHit;

    //Access the camera follow script
    private CameraFollow _cameraFollow;
    //speed threshold when the player is falling
    private float _fallSpeedYDampingChangeThreshold;
    #endregion

    #region Awake
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        _rb = GetComponent<Rigidbody2D>();
        _renderer = GetComponent<SpriteRenderer>();
        _playerCollider = GetComponent<Collider2D>();

        _cameraFollow = _cameraFollowObject.GetComponent<CameraFollow>();
    }
    #endregion

    #region Start
    private void Start()
    {
        _fallSpeedYDampingChangeThreshold = CameraManager.instance.fallSpeedYDampingChangeThreshold;
    }
    #endregion

    #region Update
    private void Update()
    {
        if (PlayerHealth.instance != null && PlayerHealth.instance.IsKnockedBack())
        {
            return;
        }

        Jump();
        FallingSpeedDamp();
    }
    #endregion

    #region FixedUpdate
    private void FixedUpdate()
    {
        // Block movement if knocked back
        if (PlayerHealth.instance != null && PlayerHealth.instance.IsKnockedBack())
        {
            return;
        }
        
        MovePlayer();
    }
    #endregion

    #region Movement Functions/Methods
    private void MovePlayer()
    {
        //get a reference to the player input script
        _moveInput = InputManager.instance.moveInput.x;

        _rb.velocity = new Vector2(_moveInput * _playerSpeed, _rb.velocity.y);

        DirCheck();
    }

    //Check in what direction the player is checking
    private void DirCheck()
    {
        if (InputManager.instance.moveInput.x > 0 && !_isFacingRight)
        {
            FlipDir();
        }
        else if (InputManager.instance.moveInput.x < 0 && _isFacingRight)
        {
            FlipDir();
        }
    }

    //flip sprite in the direction the player is moving
    private void FlipDir()
    {
        if (_isFacingRight)
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            _isFacingRight = !_isFacingRight;

            //turn the camera follow object
            _cameraFollow.CallSpriteFlip();
        }
        else
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            _isFacingRight = !_isFacingRight;

            //turn the camera follow object
            _cameraFollow.CallSpriteFlip();
        }
    }

    private void Jump()
    {
        //button was just pushed and the player is grounded
        if (InputManager.instance.playerControls.Movement.Jump.WasPressedThisFrame() && GroundCheck())
        {
            _isJumping = true;
            _rb.velocity = new Vector2(_rb.velocity.x, _jumpForce);

            //reset the jump time counter
            _jumpTimeCounter = _jumpTime;

            // Play jump SFX
            SFXManager.instance.playSFX("jump");
        }

        //button is being held
        if (InputManager.instance.playerControls.Movement.Jump.IsPressed())
        {
            if (_jumpTimeCounter > 0 && _isJumping)
            {
                _rb.velocity = new Vector2(_rb.velocity.x, _jumpForce);

                //decrease the jumptime counter (to ensure we don't float into oblivion)
                _jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                //once the timer runs out the jump ends
                _isJumping = false;
            }
        }

        //button was released this frame
        if (InputManager.instance.playerControls.Movement.Jump.WasReleasedThisFrame())
        {
            _isJumping = false;
        }

        DrawGroundCheck();
    }
    private void FallingSpeedDamp()
    {
        if (_isJumping)
        {
            CameraManager.instance.LerpYPanAndOffset(true, false);
        }
        else if (_rb.velocity.y < 0 && !_isJumping)
        {
            CameraManager.instance.LerpYPanAndOffset(false, true);
        }
        else
        {
            CameraManager.instance.LerpYPanAndOffset(false, false);
        }

    }
    #endregion

    #region GroundCheck

    private bool GroundCheck()
    {
        //create a box collider at the players feet to check if the player is grounded
        _groundHit = Physics2D.BoxCast(_playerCollider.bounds.center, _playerCollider.bounds.size, 0f, Vector2.down, _extraHeight, _groundMask);

        if (_groundHit.collider != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion

    #region Debug Function
    private void DrawGroundCheck()
    {
        Color rayColor;

        if (GroundCheck())
        {
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
        }

        Debug.DrawRay(_playerCollider.bounds.center + new Vector3(_playerCollider.bounds.extents.x, 0), Vector2.down * (_playerCollider.bounds.extents.y + _extraHeight), rayColor);
        Debug.DrawRay(_playerCollider.bounds.center - new Vector3(_playerCollider.bounds.extents.x, 0), Vector2.down * (_playerCollider.bounds.extents.y + _extraHeight), rayColor);
        Debug.DrawRay(_playerCollider.bounds.center - new Vector3(_playerCollider.bounds.extents.x, _playerCollider.bounds.extents.y + _extraHeight), Vector2.right * (_playerCollider.bounds.extents.x * 2), rayColor);
    }
    #endregion
}
