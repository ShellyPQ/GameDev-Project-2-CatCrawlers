using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    #region Variables
    public static PlayerInput instance;

    [HideInInspector] public PlayerControls playerControls;

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
        playerControls.Movement.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
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
