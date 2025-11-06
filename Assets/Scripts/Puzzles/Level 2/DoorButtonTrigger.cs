using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButtonTrigger : MonoBehaviour
{
    #region Variables

    [Header("References")]
    public OpenButtonDoor puzzleManager;
    public bool isButtonA;

    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (isButtonA)
            puzzleManager.PressButtonA();
        else
            puzzleManager.PressButtonB();
    }
}
