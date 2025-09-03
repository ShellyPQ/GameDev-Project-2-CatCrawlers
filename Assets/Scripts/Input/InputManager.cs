using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    #region Variables
    public static InputManager instance;

    [HideInInspector] public PlayerControls playerControls;

    public bool pauseMenuOpen { get; private set; }
    public bool pauseMenuClose { get; private set; }

    private InputAction _move;
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


        playerControls = new PlayerControls();
        //Get the vector 2 of in the player input and move it into my moveInput variable
        //playerControls.Movement.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();

        //cache references to actions
        _move = playerControls.Movement.Move;
        _pauseMenuOpen = playerControls.Movement.Pause;
        _pauseMenuClose = playerControls.UI.UnPause;

    }
    #endregion

    #region OnEnable and OnDisable
    private void OnEnable()
    {
        playerControls.Enable();
        _move.Enable();
        _pauseMenuOpen.Enable();
        _pauseMenuClose.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
        _move.Disable();
        _pauseMenuOpen.Disable();
        _pauseMenuClose.Disable();
    }
    #endregion

    #region Update
    private void Update()
    {
        //cache movement
        moveInput = _move.ReadValue<Vector2>();

        //pause states
        pauseMenuOpen = _pauseMenuOpen.WasPressedThisFrame();
        pauseMenuClose = _pauseMenuClose.WasPressedThisFrame();        
    }
    #endregion 
}
