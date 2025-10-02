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
    [SerializeField] private Image _sfxVoumeImage;

    [Header("Volume % Image Sprites")]
    [SerializeField] private Sprite[] _volumeSprites;

    private readonly Dictionary<string, float> _lastVolumes = new Dictionary<string, float>();
    private readonly HashSet<string> _mutedMixers = new HashSet<string>();
    #endregion

    #region Start
    private void Start()
    {
        //initialize sliders and text with current mixer values
        InitializeSlider("masterVolume", _masterSlider, _masterVolumeImage);
        InitializeSlider("musicVolume", _musicSlider, _musicVolumeImage);
        InitializeSlider("sfxVolume", _sfxSlider, _sfxVoumeImage);
    }
    #endregion

    #region Method/Functions

    #region Core Volume Logic
    private void InitializeSlider(string mixer, Slider slider, Image volumeImage)
    {
        //use slider's inspector value, not the mixer value
        float linear = slider.value;

        //apply to audiomixer
        _audioMixer.SetFloat(mixer, Mathf.Log10(Mathf.Clamp(linear, 0.0001f, 1f)) * 20f);

        //cache the initial volume
        _lastVolumes[mixer] = linear;
        
        //update volume image
        if (volumeImage != null)
        {
            int index = Mathf.Clamp(Mathf.RoundToInt(linear * 100f / 10f), 0, 10);
            volumeImage.sprite = _volumeSprites[index];
        }
    }

    private float SnapToIncrement(float value)
    {
        //snap to 10% increments
        return Mathf.Round(value * 10f) / 10f;
    }

    private void UpdateVolume(string mixer, float linearValue, Image volumeImage)
    {
        _audioMixer.SetFloat(mixer, Mathf.Log10(Mathf.Clamp(linearValue, 0.0001f, 1f)) * 20f);
        //always cache the last "active" volume
        _lastVolumes[mixer] = linearValue;
          
        if (volumeImage != null)
        {
            int index = Mathf.Clamp(Mathf.RoundToInt(linearValue * 100f / 10f), 0, 10);
            volumeImage.sprite = _volumeSprites[index];
        }
    }
   
    public void SetVolumeFromSlider(string mixer, float value, Image volumeImage)
    {
        float snapped = SnapToIncrement(value);
        UpdateVolume(mixer, snapped, volumeImage);
    }

    public void ToggleMute(string mixer, Image volumeImage, bool isMuted, Slider slider)
    {
        if (isMuted)
        {
            //cache current slider value 
            _lastVolumes[mixer] = slider.value;
            //mute (update mixer value)
            _audioMixer.SetFloat(mixer, -80f);
            _mutedMixers.Add(mixer);            

            if (volumeImage != null)
            {
                volumeImage.sprite = _volumeSprites[0];
                Debug.Log(_lastVolumes[mixer]);
            }
        }
        else
        {
            _mutedMixers.Remove(mixer);            

            //restore cached volume
            if (_lastVolumes.TryGetValue(mixer, out float cachedVolume))
            {
                slider.value = cachedVolume;
                UpdateVolume(mixer, cachedVolume, volumeImage);                
            }
        }
    }
    #endregion

    #region Button/Slider Inspector Functions/Methods
    //methods for the inspector (button bindings)
    public void SetMasterVolume(float value) => SetVolumeFromSlider("masterVolume", value, _masterVolumeImage);
    public void SetMusicVolume(float value) => SetVolumeFromSlider("musicVolume", value, _musicVolumeImage);
    public void SetSFXVolume(float value) => SetVolumeFromSlider("sfxVolume", value, _sfxVoumeImage);

    public void ToggleMasterMute(bool isMuted) => ToggleMute("masterVolume", _masterVolumeImage, isMuted, _masterSlider);
    public void ToggleMusicMute(bool isMuted) => ToggleMute("musicVolume", _musicVolumeImage, isMuted, _musicSlider);
    public void ToggleSFXMute(bool isMuted) => ToggleMute("sfxVolume", _sfxVoumeImage, isMuted, _sfxSlider);
    #endregion

    #endregion
}
