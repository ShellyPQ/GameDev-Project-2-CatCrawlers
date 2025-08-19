using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] Image _brightnessOverlay;
    [SerializeField] TextMeshProUGUI _brightnessText;
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

    public void ToggleFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    //slider value 0 - bright and 1 = dark
    public void SetBrightnessLevel(float value)
    {
        if (_brightnessOverlay != null)
        {
            Color adjColor = _brightnessOverlay.color;
            adjColor.a = value; 
            _brightnessOverlay.color = adjColor;
        }

        if (_brightnessText != null)
        {
            int percent = Mathf.RoundToInt((1f - value) * 100f);
            //invert so 0 - 0% and 1 = 100% brightness
            _brightnessText.text = percent + "%";
        }
    }

    #endregion

}
