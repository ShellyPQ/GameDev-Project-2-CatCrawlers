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

    [Header("Brightness Image Slider Properties")]
    [SerializeField] private Image _brightnessImage;
    [SerializeField] private Sprite[] _brightnessSprites;   
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
        //snap to the nearest 10% increment
        float snapped = Mathf.Round(value * 10f) / 10f;

        if (_brightnessOverlay != null)
        {
            Color adjColor = _brightnessOverlay.color;
            adjColor.a = snapped; 
            _brightnessOverlay.color = adjColor;
        }

        if (_brightnessImage != null && _brightnessSprites.Length == 11)
        {
            int index = Mathf.Clamp(Mathf.RoundToInt(snapped * 10f), 0, 10);
            _brightnessImage.sprite = _brightnessSprites[index];
        }
    }

    #endregion

}
