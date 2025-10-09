using System;
using System.Collections;
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

    #endregion

    #region Start
    private IEnumerator Start()
    {
        //wait a frame to ensure AudioManager has fully initialized
        yield return null;

        ButtonListeners();
    }
    #endregion

    #region Method/Functions

    #region Button Functions
    //When the start button is pressed
    private void StartNewGame()
    {
        StartCoroutine(PlaySFXThenLoad(() => ScenesManager.Instance.LoadNewGame()));
    }

    //When the quit button is pressed
    private void QuitGame()
    {
        StartCoroutine(PlaySFXThenLoad(() => ScenesManager.Instance.QuitGame()));
        Debug.Log("You have quit the game!");
    }

    //reset playerprefs
    private void ResetToDefault()
    {
        _brightnessSlider.value = 0f;
        _fullScreenToggle.isOn = true;

        _masterVolumeSlider.value = 0.8f;
        _sfxVolumeSlider.value = 0.8f;
        _musicVolumeSlider.value = 0.8f;

        _masterMuteToggle.isOn = false;
        _musicMuteToggle.isOn = false;
        _sfxMuteToggle.isOn = false;

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

    IEnumerator PlaySFXThenLoad(System.Action sceneChange)
    {
        //play button press sfx
        var clip = SFXManager.instance.GetClip("buttonPress");
        //get the clips length
        float waitTime = clip != null ? clip.length : 0.2f;

        //play the button press sfx
        SFXManager.instance.playSFX("buttonPress");

        //wait for the clips length
        yield return new WaitForSeconds(waitTime);

        //invoke the scene change
        sceneChange?.Invoke();
    }
    #endregion

    #region Brightness Functions   

    private void OnBrightnessChanged(float value)
    {
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
        Screen.fullScreen = isFullScreen;
    }
    #endregion

    #region Volume Slider Functions

    private void OnMasterVolumeChanged(float value)
    {
        float threshold = 0.01f;
        bool shouldMute = value <= threshold;

        _masterMuteToggle.SetIsOnWithoutNotify(shouldMute);
        _masterMuteToggle.graphic.SetAllDirty();

        _audioManager.SetMasterVolume(value);
    }

    private void OnMusicVolumeChanged(float value)
    {
        float threshold = 0.01f;
        bool shouldMute = value <= threshold;

        _musicMuteToggle.SetIsOnWithoutNotify(shouldMute);
        _musicMuteToggle.graphic.SetAllDirty();

        _audioManager.SetMusicVolume(value);
    }

    private void OnSFXVolumeChanged(float value)
    {
        float threshold = 0.01f;
        bool shouldMute = value <= threshold;

        _sfxMuteToggle.SetIsOnWithoutNotify(shouldMute);
        _sfxMuteToggle.graphic.SetAllDirty();

        _audioManager.SetSFXVolume(value);
    }

    #endregion

    #region Volume Mute Toggles
    private void OnMasterMuteChanged(bool isMuted)
    {
        _audioManager.ToggleMasterMute(isMuted);
        UpdateChildVolumeVisuals(isMuted);
    }
    private void OnMusicMuteChanged(bool isMuted)
    {
        _audioManager.ToggleMusicMute(isMuted);
    }
    private void OnSFXMuteChanged(bool isMuted)
    {
        _audioManager.ToggleSFXMute(isMuted);
    }
    public void UpdateChildVolumeVisuals(bool masterMuted)
    {
        _musicMuteToggle.SetIsOnWithoutNotify(masterMuted);
        _sfxMuteToggle.SetIsOnWithoutNotify(masterMuted);

        if (masterMuted)
        {
            _musicVolumeSlider.SetValueWithoutNotify(masterMuted ? 0f : SaveManager.LoadMusicVolume());
            _sfxVolumeSlider.SetValueWithoutNotify(masterMuted ? 0f : SaveManager.LoadSFXVolume());

            _audioManager.ToggleMusicMute(masterMuted);
            _audioManager.ToggleSFXMute(masterMuted);
        }
        else
        {
            float _musicVolume = SaveManager.LoadMusicVolume();
            float _sfxVolume = SaveManager.LoadSFXVolume();

            _musicVolumeSlider.SetValueWithoutNotify(_musicVolume);
            _sfxVolumeSlider.SetValueWithoutNotify(_sfxVolume);

            _audioManager.ToggleMusicMute(false);
            _audioManager.ToggleSFXMute(false);

            _audioManager.SetMusicVolume(_musicVolume);
            _audioManager.SetSFXVolume(_sfxVolume);
        }
        
    }
    #endregion

    #region Save Settings
    private void SaveSettings()
    {
        //save all current values
        SaveManager.SaveBrightness(_brightnessSlider.value);
        SaveManager.SaveFullscreen(_fullScreenToggle.isOn);
        SaveManager.SaveMasterVolume(_masterVolumeSlider.value);
        SaveManager.SaveMusicVolume(_musicVolumeSlider.value);
        SaveManager.SaveSFXVolume(_sfxVolumeSlider.value);
        SaveManager.SaveMasterMute(_masterMuteToggle.isOn);
        SaveManager.SaveMusicMute(_musicMuteToggle.isOn);
        SaveManager.SaveSFXMute(_sfxMuteToggle.isOn);

        Debug.Log("Settings Saved");
    }
    #endregion

    #region Load Settings
    private void LoadDefaultSettings()
    {
        //load saved volume slider values
        float _masterVolume = Mathf.Max(SaveManager.LoadMasterVolume());
        float _musicVolume = Mathf.Max(SaveManager.LoadMusicVolume());
        float _sfxVolume = Mathf.Max(SaveManager.LoadSFXVolume());

        //load saved mute values
        bool _masterVolumeMute = SaveManager.LoadMasterMute();
        bool _masterMusicMute = SaveManager.LoadMusicMute();
        bool _sfxMute = SaveManager.LoadSFXMute();

        //update audio sliders
        _masterVolumeSlider.SetValueWithoutNotify(_masterVolume);
        _musicVolumeSlider.SetValueWithoutNotify(_musicVolume);
        _sfxVolumeSlider.SetValueWithoutNotify(_sfxVolume);

        _audioManager.SetMasterVolume(_masterVolume);
        _audioManager.SetMusicVolume(_musicVolume);
        _audioManager.SetSFXVolume(_sfxVolume);

        //update AudioManager with saved values
        _audioManager.ToggleMasterMute(_masterVolumeMute);
        _audioManager.ToggleMusicMute(_masterMusicMute);
        _audioManager.ToggleSFXMute(_sfxMute);      

        //update audio mute toggles
        _masterMuteToggle.SetIsOnWithoutNotify(_masterVolumeMute);
        _musicMuteToggle.SetIsOnWithoutNotify(_masterMusicMute);
        _sfxMuteToggle.SetIsOnWithoutNotify(_sfxMute);

        //load brightness and fullscreen
        _brightnessSlider.SetValueWithoutNotify(SaveManager.LoadDefaultBrightness());
        SetBrightnessLevel(_brightnessSlider.value);
        _fullScreenToggle.SetIsOnWithoutNotify(SaveManager.LoadFullscreen());
    }
    #endregion

    #region Update Settings UI
    private void ApplyTempSettingsToUI()
    {
        _audioManager.SetMasterVolume(_masterVolumeSlider.value);
        _audioManager.SetMusicVolume(_musicVolumeSlider.value);
        _audioManager.SetSFXVolume(_sfxVolumeSlider.value);

        _audioManager.ToggleMasterMute(_masterMuteToggle.isOn);
        _audioManager.ToggleMusicMute(_musicMuteToggle.isOn);
        _audioManager.ToggleSFXMute(_sfxMuteToggle.isOn);
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
