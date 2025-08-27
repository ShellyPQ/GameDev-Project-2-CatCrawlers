using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleePracticeDummy : MonoBehaviour
{
    #region Variables
    [Header("References")]
    //Reference to player to access attack script
    private GameObject _player;
    #endregion

    #region Awake
    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }
    #endregion

    #region Start
    private void Start()
    {
        //disable the players attack at the start of the tutorial
        _player.gameObject.GetComponent<PlayerAttack>().enabled = false;
    }
    #endregion
}
