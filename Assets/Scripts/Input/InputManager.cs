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

    private InputAction _move;
    private InputAction _pauseMenuOpen;
    private InputAction _pauseMenuClose;
    private InputAction _attackMelee;
    private InputAction _attackRange;

    public bool pauseMenuOpen { get; private set; }
    public bool pauseMenuClose { get; private set; }
    public bool attackMelee { get; private set; }
    public bool attackRange { get; private set; }

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
            _attackMelee = playerControls.Movement.AttackMelee;
            _attackRange = playerControls.Movement.AttackRange;

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
        
        //ensure main menu and level select always us have the UI action map active
        if (scene.name == ScenesManager.Scene.MainMenu.ToString() || scene.name == ScenesManager.Scene.Level_Select_Scene.ToString() || scene.name == ScenesManager.Scene.Intro_Cutscene.ToString())
        {
            EnableUI();
        }
        else
        {
            EnableMovement();

            if (PlayerHealth.instance != null)
            {
                PlayerHealth.instance.ResetPlayerCollider();
            }
        }

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
        attackMelee = _attackMelee.WasPressedThisFrame();
        attackRange = _attackRange.WasPressedThisFrame();
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

            //hide and lock cursor during gameplay
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
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

            //show and unlock cursor in UI
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    #endregion
}
