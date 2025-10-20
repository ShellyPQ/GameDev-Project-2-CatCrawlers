using System.ComponentModel;
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
    //[SerializeField] private float _jumpTime = 0.5f;

    [Header("Double Jump Properties")]
    [Tooltip("Player Jumpforce when jumping again during double jump.")]
    [SerializeField] private float _doubleJumpForce = 3f;
    [Tooltip("Particle System Prefab used during double jump.")]
    [SerializeField] private ParticleSystem _doubleJumpParticle;

    private bool _hasDoubleJumped;

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
    private Animator _ani;

    //Movement and jump states
    private float _moveInput;
    public bool _isFacingRight = true;

    //in an if statement, used to clear out warning in unity console
#pragma warning disable 0414
    private bool _isJumping = false;
#pragma warning restore 0414

    //private bool _wasJumping = false;
    //private bool _wasFalling = false;

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
        _ani = GetComponent<Animator>();
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

        #region Double Jump Grounded Check

        bool isGrounded = GroundCheck();

        //reset coyote timer if player is grounded
        if (isGrounded)
        {
            _hasDoubleJumped = false;
        }

        #endregion
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
        #region Movement Animation Logic

        _ani.SetFloat("speed", Mathf.Abs(_rb.velocity.x));
        _ani.SetBool("isGrounded", GroundCheck());

        #endregion

        ApplyExtraLift();
        #region Jump Animation Logic

        _ani.SetFloat("verticalVelocity", _rb.velocity.y);

        if (!GroundCheck())
        {
            float yVel = Mathf.Abs(_rb.velocity.y);
            _ani.speed = Mathf.Clamp(0.8f + yVel / 15f, 0.8f, 1.3f);
        }
        else
        {
            //reset speed when grounded
            _ani.speed = 1f;
        }

        //if grounded but animation still in air, force it back to idle
        if (GroundCheck() && _ani.GetCurrentAnimatorStateInfo(0).IsName("Jump_Float"))
        {
            //cancel the jump trigger
            _ani.ResetTrigger("jumpPressed");
            //force transition to locomotion blend tree
            _ani.CrossFade("Locomotion", 0.05f);
        }
        #endregion
    }
    #endregion

    #region LateUpdate
    private void LateUpdate()
    {
        bool isGrounded = GroundCheck();
        //smooth camera pan based on Y velocity
        CameraManager.instance.smoothFollowY(_rb.velocity.y, isGrounded);
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
        bool jumpPressed = InputManager.instance.playerControls.Movement.Jump.WasPressedThisFrame();
        bool jumpReleased = InputManager.instance.playerControls.Movement.Jump.WasReleasedThisFrame();
        bool isGrounded = GroundCheck();

        //normal jump
        if (jumpPressed && isGrounded)
        {
            PerformJump(_jumpForce);
            return;
        }

        //coyote jump
        if (jumpPressed && !isGrounded && !_hasDoubleJumped)
        {
            PerformJump(_doubleJumpForce);
            _hasDoubleJumped = true;

            //play particles effect
            if (_doubleJumpParticle != null)
            {
                Vector3 spawnPos = new Vector3(_playerCollider.bounds.center.x, _playerCollider.bounds.center.y - .5f, 0);

                ParticleSystem particles = Instantiate(_doubleJumpParticle, spawnPos, Quaternion.identity);
                particles.Play();
                Destroy(particles.gameObject, particles.main.duration + particles.main.startLifetime.constantMax);
            }
            
            return;
        }

        if (jumpReleased)
        {
            _isJumping = false;
        }

        ////button was pressed and the player is grounded
        //if (InputManager.instance.playerControls.Movement.Jump.WasPressedThisFrame() && GroundCheck())
        //{
        //    _rb.velocity = new Vector2(_rb.velocity.x, _jumpForce);
        //    _isJumping = true;

        //    //trigger jump_start animation
        //    _ani.SetTrigger("jumpPressed");

        //    // Play jump SFX
        //    SFXManager.instance.playSFX("jump");
        //}

        ////button was released this frame
        //if (InputManager.instance.playerControls.Movement.Jump.WasReleasedThisFrame())
        //{
        //    _isJumping = false;
        //}

        DrawGroundCheck();
    }

    private void PerformJump(float jumpStrength)
    {
        _rb.velocity = new Vector2(_rb.velocity.x, jumpStrength);
        _isJumping = true;

        _ani.SetTrigger("jumpPressed");
        SFXManager.instance.playSFX("jump");
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
        //create a box collider around the players feet only
        Vector2 boxCenter = new Vector2(_playerCollider.bounds.center.x, _playerCollider.bounds.min.y + 0.05f);
        Vector2 boxSize = new Vector2(_playerCollider.bounds.size.x * 0.9f, 0.1f);

        _groundHit = Physics2D.BoxCast(boxCenter, boxSize, 0f, Vector2.down, _extraHeight, _groundMask);

        return _groundHit.collider != null;
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
