using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioMixer _audioMixer;

    #region Variables
    [Header("Slider References")]
    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;

    [Header("Volume Images")]
    [SerializeField] private Image _masterVolumeImage;
    [SerializeField] private Image _musicVolumeImage;
    [SerializeField] private Image _sfxVolumeImage;

    [Header("Volume % Image Sprites")]
    [SerializeField] private Sprite[] _volumeSprites;

    private readonly Dictionary<string, float> _lastVolumes = new Dictionary<string, float>();
    private bool _isUpdatingSlider = false;
    #endregion

    #region Start
    private void Start()
    {
        //store initial values to ensure mute/unmute restores properly
        _lastVolumes["masterVolume"] = _masterSlider.value;
        _lastVolumes["musicVolume"] = _musicSlider.value;
        _lastVolumes["sfxVolume"] = _sfxSlider.value;

        //initialize sliders and text with current mixer values
        InitializeSlider("masterVolume", _masterSlider, _masterVolumeImage);
        InitializeSlider("musicVolume", _musicSlider, _musicVolumeImage);
        InitializeSlider("sfxVolume", _sfxSlider, _sfxVolumeImage);

        //set the initial mixer values
        ApplyVolume("masterVolume", _masterSlider.value);
        ApplyVolume("musicVolume", _musicSlider.value);
        ApplyVolume("sfxVolume", _sfxSlider.value);

        // Debug what the mixer really holds
        float masterDb, musicDb, sfxDb;
        _audioMixer.GetFloat("masterVolume", out masterDb);
        _audioMixer.GetFloat("musicVolume", out musicDb);
        _audioMixer.GetFloat("sfxVolume", out sfxDb);
    }
    #endregion

    #region Method/Functions

    #region Core Volume Logic
    private void InitializeSlider(string mixer, Slider slider, Image volumeImage)
    {
        //use slider's inspector value, not the mixer value
        float initialValue = slider.value;
        _lastVolumes[mixer] = initialValue;
        UpdateSlider(slider, volumeImage, initialValue);
    }

    private float SnapToIncrement(float value)
    {
        //snap to 10% increments
        return Mathf.Round(value * 10f) / 10f;
    }

    private void ApplyVolume(string mixer, float linearValue)
    {
        float clampedValue = Mathf.Clamp(linearValue, 0.0001f, 1f);
        float db = Mathf.Log10(clampedValue) * 20f;
        _audioMixer.SetFloat(mixer, db);
    }

    private void UpdateSlider(Slider slider, Image volumeImage, float value, bool notify = true)
    {
        if (slider != null)
        {
            if (notify)
            {
                slider.value = value;
            }
            else
            {
                slider.SetValueWithoutNotify(value);
            }
        }

        if (volumeImage != null && _volumeSprites.Length == 11)
        {
            int index = Mathf.Clamp(Mathf.RoundToInt(value * 10f), 0, 10);
            volumeImage.sprite = _volumeSprites[index];
        }
    }
    public void SetVolumeFromSlider(string mixer, float value, Image volumeImage, Toggle muteToggle = null)
    {
        if (_isUpdatingSlider)
        {
            return;
        }

        _isUpdatingSlider = true;

        float snapped = SnapToIncrement(value);

        //if slider is moved when muted, unmute 
        if (muteToggle != null && muteToggle.isOn && snapped > 0f)
        {
            muteToggle.isOn = false;
        }

        if (snapped > 0f)
        {
            _lastVolumes[mixer] = snapped;
        }

        ApplyVolume(mixer, snapped);
        UpdateSlider(GetSliderForMixer(mixer), volumeImage, snapped);

        _isUpdatingSlider = false;
    }

    public void ToggleMute(string mixer, Image volumeImage, bool isMuted, Slider slider = null)
    {
        slider ??= GetSliderForMixer(mixer);

        if (slider == null)
        {
            return;
        }

        if (isMuted)
        {
            _lastVolumes[mixer] = slider.value > 0 ? slider.value : (_lastVolumes.ContainsKey(mixer) ? _lastVolumes[mixer] : 0.8f);

            ApplyVolume(mixer, 0f);
            UpdateSlider(slider, volumeImage, 0f, false);
        }
        else
        {
            //restore volume from last stored value
            float restoredVolume = _lastVolumes.ContainsKey(mixer) ? _lastVolumes[mixer] : 0.8f;
            ApplyVolume(mixer, restoredVolume);
            slider.SetValueWithoutNotify(restoredVolume);
            UpdateSlider(slider, volumeImage, restoredVolume, false);
        }
    }

    private Slider GetSliderForMixer(string mixer)
    {
        return mixer switch
        {
            "masterVolume" => _masterSlider,
            "musicVolume" => _musicSlider,
            "sfxVolume" => _sfxSlider,
            _ => null
        };
    }

    public void UpdateVolumeMuteImage(string mixer, float value) 
    {
        Image toggleImage = mixer switch
        {
            "masterVolume" => _masterVolumeImage,
            "musicVolume" => _musicVolumeImage,
            "sfxVolume" => _sfxVolumeImage,
            _ => null
        };

        if (toggleImage != null && _volumeSprites.Length == 11)
        {
            int index = Mathf.Clamp(Mathf.RoundToInt(value * 10f), 0, 10);
            toggleImage.sprite = _volumeSprites[index];
        }
    }
    #endregion

    #region Button/Slider Inspector Functions/Methods
    //methods for the inspector (button bindings)
    public void SetMasterVolume(float value) => SetVolumeFromSlider("masterVolume", value, _masterVolumeImage);
    public void SetMusicVolume(float value) => SetVolumeFromSlider("musicVolume", value, _musicVolumeImage);
    public void SetSFXVolume(float value) => SetVolumeFromSlider("sfxVolume", value, _sfxVolumeImage);

    public void ToggleMasterMute(bool isMuted) => ToggleMute("masterVolume", _masterVolumeImage, isMuted);
    public void ToggleMusicMute(bool isMuted) => ToggleMute("musicVolume", _musicVolumeImage, isMuted);
    public void ToggleSFXMute(bool isMuted) => ToggleMute("sfxVolume", _sfxVolumeImage, isMuted);

    #endregion

    #endregion
}
