using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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

            playerControls = new PlayerControls();

            //cache references to actions
            _move = playerControls.Movement.Move;
            _pauseMenuOpen = playerControls.Movement.Pause;
            _pauseMenuClose = playerControls.UI.UnPause;

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            //ensure there is only one of this
            Destroy(gameObject);
        }
    }
    #endregion

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        //reset when entering a new scene
        Time.timeScale = 1f;
        EnableMovement();

    }

    #region OnEnable and OnDisable
    private void OnEnable()
    {
        //playerControls.Enable();
        EnableMovement();
    }

    private void OnDisable()
    {
        playerControls.Disable();
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

    #region Movement and UI mapping swap method/functions
    
    //enable movement map
    public void EnableMovement()
    {
        if (!playerControls.Movement.enabled)
        {
            //clear this to allow us to set the map to prevent memory leak issues
            playerControls.Disable();
            //only enable the movement map
            playerControls.Movement.Enable();
        }      
    }

    //enable UI map
    public void EnableUI()
    {
        if (!playerControls.UI.enabled)
        {
            //clear this to allow us to set the map to prevent memory leak issues
            playerControls.Disable();
            //only enable the ui map
            playerControls.UI.Enable();
        }
    }

    #endregion
}
