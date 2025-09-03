using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Variables
    [Header("Panels")]
    [SerializeField] private GameObject _mainMenuPanel;
    [SerializeField] private GameObject _settingsPanel;

    [Header("Buttons")]
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _quitButton;
    [SerializeField] private Button _defaultButton;
    [SerializeField] private Button _saveButton;
    [SerializeField] private Button _backButton;

    [Header("Brightness")]
    [SerializeField] private Image _brightnessOverlay;
    [SerializeField] private Image _brightnessImage;
    [SerializeField] private Sprite[] _brightnessSprites;
    [SerializeField] private Slider _brightnessSlider;

    [Header("Fullscreen")]
    [SerializeField] private Toggle _fullScreenToggle;

    [Header("Volume Sliders")]
    [SerializeField] private Slider _masterVolumeSlider;
    [SerializeField] private Slider _sfxVolumeSlider;
    [SerializeField] private Slider _musicVolumeSlider;

    [Header("Volume Mute Toggles")]
    [SerializeField] private Toggle _masterMuteToggle;
    [SerializeField] private Toggle _musicMuteToggle;
    [SerializeField] private Toggle _sfxMuteToggle;

    [Header("Script References")]
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private MenuEventManager _menuEventManager;


    //temporary storage for unsaved settings
    private float _tempBrightness;
    private bool _tempFullScreen;
    private float _tempMasterVolume;
    private float _tempMusicVolume;
    private float _tempSFXVolume;
    private bool _tempMasterMute;
    private bool _tempMusicMute;
    private bool _tempSFXMute;
    #endregion

    #region Start
    private void Start()
    {
        ButtonListeners();
    }
    #endregion

    #region Method/Functions

    #region Button Functions
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

    //reset playerprefs
    private void ResetToDefault()
    {
        //hardcoded defaults
        _tempBrightness = 0f;
        _tempFullScreen = true;
        _tempMasterVolume = 0.8f;
        _tempMusicVolume = 0.8f;
        _tempSFXVolume = 0.8f;
        _tempMasterMute = false;
        _tempMusicMute = false;
        _tempSFXMute = false;

        ApplyTempSettingsToUI();
    }

    private void BackToMainMenu()
    {
        //kill any active tweens
        _menuEventManager?.KillAllTweens();

        //hide setting panel
        _settingsPanel.SetActive(false);
        _mainMenuPanel.SetActive(true);

        //discard unsaved changes, reload saved values
        LoadDefaultSettings();
    }

    public void OpenSettingsPanel()
    {
        //kill any active tweens
        _menuEventManager?.KillAllTweens();

        _settingsPanel.SetActive(true);
        _mainMenuPanel.SetActive(false);

        //load saved values into UI
        LoadDefaultSettings();
    }
    #endregion

    #region Brightness Functions   

    private void OnBrightnessChanged(float value)
    {
        _tempBrightness = value;
        SetBrightnessLevel(value);
    }
    public void SetBrightnessLevel(float value)
    {
        //slider value 0 - bright and 1 = dark
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

    #region Fullscreen Function
    public void ToggleFullScreen(bool isFullScreen)
    {
        _tempFullScreen = isFullScreen;
        Screen.fullScreen = isFullScreen;
    }
    #endregion

    #region Volume Slider Functions

    private void OnMasterVolumeChanged(float value)
    {
        _tempMasterVolume = value;
        _audioManager.SetMasterVolume(value);
    }

    private void OnMusicVolumeChanged(float value)
    {
        _tempMusicVolume = value;
        _audioManager.SetMusicVolume(value);
    }

    private void OnSFXVolumeChanged(float value)
    {
        _tempSFXVolume = value;
        _audioManager.SetSFXVolume(value);
    }

    #endregion

    #region Volume Mute Toggles
    private void OnMasterMuteChanged(bool isMuted)
    {
        _tempMasterMute = isMuted;
        _audioManager.ToggleMasterMute(isMuted);
    }
    private void OnMusicMuteChanged(bool isMuted)
    {
        _tempMusicMute = isMuted;
        _audioManager.ToggleMusicMute(isMuted);
    }
    private void OnSFXMuteChanged(bool isMuted)
    {
        _tempSFXMute = isMuted;
        _audioManager.ToggleSFXMute(isMuted);
    }
    #endregion

    #region Save Settings
    private void SaveSettings()
    {
        //save all current values
        SaveManager.SaveBrightness(_tempBrightness);
        SaveManager.SaveFullscreen(_tempFullScreen);
        SaveManager.SaveMasterVolume(_tempMasterVolume);
        SaveManager.SaveMasterVolume(_tempMusicVolume);
        SaveManager.SaveSFXVolume(_tempSFXVolume);
        SaveManager.SaveMasterMute(_tempMasterMute);
        SaveManager.SaveMusicMute(_tempMusicMute);
        SaveManager.SaveSFXMute(_tempSFXMute);

        Debug.Log("Settings Saved");
    }
    #endregion

    #region Load Settings
    private void LoadDefaultSettings()
    {
        //brightness
        _tempBrightness = SaveManager.LoadDefaultBrightness();
        _brightnessSlider.value = _tempBrightness;
        SetBrightnessLevel(_tempBrightness);

        //fullscreen
        _tempFullScreen = SaveManager.LoadFullscreen();
        _fullScreenToggle.isOn = _tempFullScreen;
        Screen.fullScreen = _tempFullScreen;

        //audio sliders
        _tempMasterVolume = SaveManager.LoadMasterVolume();
        _tempMusicVolume = SaveManager.LoadMasterVolume();
        _tempSFXVolume = SaveManager.LoadSFXVolume();

        _masterVolumeSlider.value = _tempMasterVolume;
        _musicVolumeSlider.value = _tempMusicVolume;
        _sfxVolumeSlider.value = _tempSFXVolume;

        _audioManager.SetMasterVolume(_tempMasterVolume);
        _audioManager.SetMusicVolume(_tempMusicVolume);
        _audioManager.SetSFXVolume(_tempSFXVolume);

        //audio mute toggles
        _tempMasterMute = SaveManager.LoadMasterMute();
        _tempMusicMute = SaveManager.LoadMusicMute();
        _tempSFXMute = SaveManager.LoadSFXMute();

        _masterMuteToggle.isOn = _tempMasterMute;
        _musicMuteToggle.isOn = _tempMusicMute;
        _sfxMuteToggle.isOn = _tempSFXMute;

        _audioManager.ToggleMasterMute( _tempMasterMute);
        _audioManager.ToggleMusicMute(_tempMusicMute);
        _audioManager.ToggleSFXMute(_tempSFXMute);
    }
    #endregion

    #region Update Settings UI
    private void ApplyTempSettingsToUI()
    {
        _brightnessSlider.value=_tempBrightness;
        SetBrightnessLevel(_tempBrightness);

        _fullScreenToggle.isOn = _tempFullScreen;
        Screen.fullScreen = _tempFullScreen;

        _masterVolumeSlider.value = _tempMasterVolume;
        _musicVolumeSlider.value = _tempMusicVolume;
        _sfxVolumeSlider.value = _tempSFXVolume;

        _audioManager.SetMasterVolume(_tempMasterVolume);
        _audioManager.SetMusicVolume(_tempMusicVolume);
        _audioManager.SetSFXVolume(_tempSFXVolume);

        _audioManager.ToggleMasterMute(_tempMasterMute);
        _audioManager.ToggleMusicMute(_tempMusicMute);
        _audioManager.ToggleSFXMute(_tempSFXMute);
    }
    #endregion

    #region Event Listeners
    private void ButtonListeners()
    {
        //null checks as I may use this script in other scenes
        // Buttons
        if (_startButton != null) _startButton.onClick.AddListener(StartNewGame);
        if (_quitButton != null) _quitButton.onClick.AddListener(QuitGame);
        if (_defaultButton != null) _defaultButton.onClick.AddListener(ResetToDefault);
        if (_saveButton != null) _saveButton.onClick.AddListener(SaveSettings);
        if (_backButton != null) _backButton.onClick.AddListener(BackToMainMenu);

        // Sliders
        if (_brightnessSlider != null) _brightnessSlider.onValueChanged.AddListener(OnBrightnessChanged);
        if (_masterVolumeSlider != null) _masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        if (_musicVolumeSlider != null) _musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        if (_sfxVolumeSlider != null) _sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        // Toggles
        if (_fullScreenToggle != null) _fullScreenToggle.onValueChanged.AddListener(ToggleFullScreen);
        if (_masterMuteToggle != null) _masterMuteToggle.onValueChanged.AddListener(OnMasterMuteChanged);
        if (_musicMuteToggle != null) _musicMuteToggle.onValueChanged.AddListener(OnMusicMuteChanged);
        if (_sfxMuteToggle != null) _sfxMuteToggle.onValueChanged.AddListener(OnSFXMuteChanged);

        // Load saved settings into UI & AudioManager
        LoadDefaultSettings();
    }

    #endregion

    #endregion
}
