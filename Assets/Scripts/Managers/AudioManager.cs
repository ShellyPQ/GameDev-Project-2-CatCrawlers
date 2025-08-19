using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioMixer _audioMixer;

    #region Variables
    [Header("Slider References")]
    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;

    [Header("Text References")]
    [SerializeField] private TextMeshProUGUI _masterVolumeText;
    [SerializeField] private TextMeshProUGUI _musicVolumeText;
    [SerializeField] private TextMeshProUGUI _sfxVolumeText;

    private readonly Dictionary<string, float> _lastVolumes = new Dictionary<string, float>();
    private readonly HashSet<string> _mutedMixers = new HashSet<string>();
    #endregion

    #region Start
    private void Start()
    {
        //Initialize sliders and text with current mixer values
        InitializeSlider("masterVolume", _masterSlider, _masterVolumeText);
        InitializeSlider("musicVolume", _musicSlider, _musicVolumeText);
        InitializeSlider("sfxVolume", _sfxSlider, _sfxVolumeText);
    }
    #endregion

    #region Method/Functions

    #region Set and Mute Volume Logic
    private void InitializeSlider(string mixer, Slider slider, TextMeshProUGUI volumeText)
    {
        _audioMixer.GetFloat(mixer, out float dB);
        float linear = Mathf.Pow(10, dB / 20);
        _lastVolumes[mixer] = linear;

        slider.value = linear;
        volumeText.text = Mathf.RoundToInt(linear * 100f) + "%";
    }
    public void SetVolume(string mixer, float linearValue, TextMeshProUGUI volumeText)
    {
        if (_mutedMixers.Contains(mixer))
        {
            return;
        }

        _audioMixer.SetFloat(mixer, Mathf.Log10(Mathf.Clamp(linearValue, 0.0001f, 1f)) * 20f);
        _lastVolumes[mixer] = linearValue;

        if (volumeText != null)
        {
            volumeText.text = Mathf.RoundToInt(linearValue * 100f) + "%";
        }
    }
    public void ToggleMute(string mixer, TextMeshProUGUI volumeText, bool isMuted, Slider slider)
    {
        if (isMuted)
        {
            //cashe current slider value 
            _lastVolumes[mixer] = slider.value;

            //mute
            _audioMixer.SetFloat(mixer, -80f);
            _mutedMixers.Add(mixer);
            volumeText.text = "0%";
        }
        else
        {
            //restore cached volume
            if (_lastVolumes.TryGetValue(mixer, out float linearVol))
            {
                _audioMixer.SetFloat(mixer, Mathf.Log10(Mathf.Clamp(linearVol, 0.0001f, 1f)) * 20);
                slider.value = linearVol;
                volumeText.text = Mathf.RoundToInt(linearVol * 100f) + "%";
            }

            _mutedMixers.Remove(mixer);
        }
    }
    #endregion

    #region Button/Slider Inspector Functions/Methods
    //Methods for the inspector (button bindings)
    public void SetMasterVolume(float value) => SetVolume("masterVolume", value, _masterVolumeText);
    public void SetMusicVolume(float value) => SetVolume("musicVolume", value, _musicVolumeText);
    public void SetSFXVolume(float value) => SetVolume("sfxVolume", value, _sfxVolumeText);

    public void ToggleMasterMute(bool isMuted) => ToggleMute("masterVolume", _masterVolumeText, isMuted, _masterSlider);
    public void ToggleMusicMute(bool isMuted) => ToggleMute("musicVolume", _musicVolumeText, isMuted, _musicSlider);
    public void ToggleSFXMute(bool isMuted) => ToggleMute("sfxVolume", _sfxVolumeText, isMuted, _sfxSlider);
    #endregion

    #endregion
}
