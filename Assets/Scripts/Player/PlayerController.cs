using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables
    public static PlayerController instance;

    [Header("Movement Properties")]
    [Tooltip("Player movements speed")]
    [SerializeField] private float _playerSpeed = 5f;

    [Header("Jump Properties")]
    [SerializeField] private float _jumpForce = 19f;
    [SerializeField] private float _jumpTime = 0.5f;

    [Header("Ground Check Properties")]
    [SerializeField] private float _extraHeight = 0.35f;
    [SerializeField] private LayerMask _groundMask;

    [Header("Gravity Modifiers")]
    [Tooltip("Makes falling faster but smooth")]
    [SerializeField] private float _fallMultiplier = 1.1f;
    [Tooltip("Modifier used to taper off the jump")]
    [SerializeField] private float _lowJumpMultiplier = 1.1f;
    [Tooltip("reduce gravity while holding jump")]
    [SerializeField] private float _gravityMultiplier = 0.7f;

    [Header("Camera Properties")]
    [SerializeField] private GameObject _cameraFollowObject;

    //References
    private SpriteRenderer _renderer;
    private Rigidbody2D _rb;
    private Collider2D _playerCollider;
    private CameraFollow _cameraFollow;

    //Movement and jump states
    private float _moveInput;
    public bool _isFacingRight = true;
    private bool _isJumping;
    private bool _wasJumping;
    private bool _wasFalling;

    private RaycastHit2D _groundHit;

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

    #region Update
    private void Update()
    {
        if (PlayerHealth.instance != null && PlayerHealth.instance.IsKnockedBack())
        {
            return;
        }

        Jump();
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
        ApplyExtraLift();
    }
    #endregion

    #region LateUpdate
    private void LateUpdate()
    {
        //smooth camera pan based on Y velocity
        CameraManager.instance.smoothFollowY(_rb.velocity.y);
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
        //button was pressed and the player is grounded
        if (InputManager.instance.playerControls.Movement.Jump.WasPressedThisFrame() && GroundCheck())
        {
            _rb.velocity = new Vector2(_rb.velocity.x, _jumpForce);
            _isJumping = true;

            // Play jump SFX
            SFXManager.instance.playSFX("jump");
        }

        //button was released this frame
        if (InputManager.instance.playerControls.Movement.Jump.WasReleasedThisFrame())
        {
            _isJumping = false;
        }

        DrawGroundCheck();
    }

    private void ApplyExtraLift()
    {
        //if the player is falling
        if (_rb.velocity.y < 0)
        {
            _rb.velocity += Vector2.up * Physics2D.gravity.y * (_fallMultiplier - 1) * Time.deltaTime;
        }
        //if holding the jump
        else if (_rb.velocity.y > 0 && InputManager.instance.playerControls.Movement.Jump.IsPressed())
        {
            _rb.velocity += Vector2.up * Physics2D.gravity.y * (_gravityMultiplier - 1) * Time.deltaTime;
        }
        //if jump is released early
        else if (_rb.velocity.y > 0 && !InputManager.instance.playerControls.Movement.Jump.IsPressed())
        {
            _rb.velocity += Vector2.up * Physics2D.gravity.y * (_lowJumpMultiplier - 1) * Time.deltaTime;
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
