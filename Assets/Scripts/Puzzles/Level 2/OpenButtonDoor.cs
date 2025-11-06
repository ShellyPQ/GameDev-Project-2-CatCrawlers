using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenButtonDoor : MonoBehaviour
{
    #region variables

    [Header("Button Animators")]
    [SerializeField] private Animator _buttonAnimatorA;
    [SerializeField] private Animator _buttonAnimatorB;

    [Header("Door Animators")]
    [SerializeField] private Animator _door1Ani;
    [SerializeField] private Animator _door2Ani;

    private bool _buttonAPress = false;
    private bool _buttonBPressed = false;

    #endregion

    #region Mathod/Functions

    public void PressButtonA() 
    {
        _buttonAPress = true;
        _buttonAnimatorA.SetBool("ButtonPressed", true);

        //reset B if pressed before A
        if (!_buttonBPressed)
        {
            _buttonAnimatorB.SetBool("ButtonPressed", false);
        }
    }

    public void PressButtonB()
    {
        //animate always
        _buttonAnimatorB.SetBool("ButtonPressed", true);

        //only count if A already pressed
        if (_buttonAPress)
        {
            _buttonBPressed = true;
            CheckPuzzle();
        }
        else
        {
            //wrong order reset button
            StartCoroutine(ResetB());
        }
    }

    private void CheckPuzzle()
    {
        if (_buttonAPress && _buttonBPressed)
        {
            _door1Ani.SetBool("canProceed", true);
            _door2Ani.SetBool("canProceed", true);

            //disable the colliders on the door
            _door1Ani.gameObject.GetComponent<Collider2D>().enabled = false;
            _door2Ani.gameObject.GetComponent<Collider2D>().enabled = false;
        }
    }

    #endregion

    #region Coroutine
    private IEnumerator ResetB()
    {
        yield return new WaitForSeconds(0.4f);
        _buttonAnimatorB.SetBool("ButtonPressed", false);
    }

    #endregion
}
