using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Variables
    [Header("References")]
    [Tooltip("Variable to hold the start button.")]
    [SerializeField] Button _startButton;
    [Tooltip("Variable to hold the quit button.")]
    [SerializeField] Button _quitButton;
    #endregion

    #region Start
    private void Start()
    {
        //When the start button is selected - start new game
        _startButton.onClick.AddListener(StartNewGame);
        //When the quit button is selected - quit the game
        _quitButton.onClick.AddListener(QuitGame);
    }
    #endregion

    #region Method/Functions
    //When the start button is pressed
    private void StartNewGame()
    {
        ScenesManager.Instance.LoadNewGame();
    }

    //When the quit button is pressed
    private void QuitGame()
    {
        ScenesManager.Instance.QuitGame();
        Debug.Log("You have quit the game!");
    }

    #endregion

}
