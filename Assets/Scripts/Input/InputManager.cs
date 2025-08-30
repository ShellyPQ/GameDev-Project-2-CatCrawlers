using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    #region Variables
    public static InputManager instance;

    public static PlayerInput playerInput;

    [HideInInspector] public PlayerControls playerControls;

    public bool pauseMenuOpen { get; private set; }
    public bool pauseMenuClose { get; private set; }

    private InputAction _pauseMenuOpen;
    private InputAction _pauseMenuClose;

    //this will hold the move input vector
    [HideInInspector] public Vector2 moveInput;
    #endregion

    #region Awake
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            //ensure there is only one of this
            Destroy(gameObject);
        }

        playerInput = GetComponent<PlayerInput>();

        playerControls = new PlayerControls();
        //Get the vector 2 of in the player input and move it into my moveInput variable
        playerControls.Movement.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        
        _pauseMenuOpen = playerInput.actions["Pause"];
        _pauseMenuClose = playerInput.actions["UnPause"];
    }
    #endregion

    #region Update
    private void Update()
    {
        pauseMenuOpen = _pauseMenuOpen.WasPressedThisFrame();
        pauseMenuClose = _pauseMenuClose.WasPressedThisFrame();
    }
    #endregion

    #region OnEnable and OnDisable
    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }
    #endregion
}
