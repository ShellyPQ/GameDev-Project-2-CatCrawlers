using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    #region Variables
    //Keys
    private const string BrightnessKey = "brightness";
    private const string MasterVolumeKey = "volume_master";
    private const string SFXVolumeKey = "volume_sfx";
    private const string MusicVolumeKey = "volume_music";
    private const string MasterMuteKey = "mute_master";
    private const string MusicMuteKey = "mute_music";
    private const string SFXMuteKey = "mute_sfx";
    private const string FullscreenKey = "fullscreen";

    //settingsdefault values
    private const float DefaultBrightness = 0;    
    private const float DefaultMasterVolume = 0.8f;
    private const float DefaultSFXVolume = 0.8f;
    private const float DefaultMusicVolume = 0.8f;
    private const bool DefaultFullscreen = true;
    #endregion

    #region Method/Functions

    #region Brightness Save/Load Function
    public static void SaveBrightness(float value)
    {
        PlayerPrefs.SetFloat(BrightnessKey, value);
    }

    public static float LoadDefaultBrightness()
    {
        return PlayerPrefs.GetFloat(BrightnessKey, DefaultBrightness);
    }
    #endregion

    #region Volume Save/Load Function

    #region Master Volume Slider and Mute Toggle
    public static void SaveMasterVolume(float value)
    {
        PlayerPrefs.SetFloat(MasterVolumeKey, value);
    }

    public static float LoadMasterVolume()
    {
        return PlayerPrefs.GetFloat(MasterVolumeKey, DefaultMasterVolume);
    }

    public static void SaveMasterMute(bool isMuted)
    {
        PlayerPrefs.SetInt(MasterMuteKey, isMuted ? 1 : 0);
    }

    public static bool LoadMasterMute()
    {
       return PlayerPrefs.GetInt(MasterMuteKey, 0) == 1;
    }

    #endregion

    #region SFX Volume and Mute Toggle
    public static void SaveSFXVolume(float value)
    {
        PlayerPrefs.SetFloat(SFXVolumeKey, value);
    }

    public static float LoadSFXVolume()
    {
        return PlayerPrefs.GetFloat(SFXVolumeKey, DefaultSFXVolume);
    }

    public static void SaveSFXMute(bool isMuted)
    {
        PlayerPrefs.SetInt(SFXMuteKey, isMuted ? 1 : 0);
    }

    public static bool LoadSFXMute()
    {
        return PlayerPrefs.GetInt(SFXMuteKey, 0) == 1;
    }
    #endregion

    #region Music Volume and Mute Toggle
    public static void SaveMusicVolume(float value)
    {
        PlayerPrefs.SetFloat(MusicVolumeKey, value);
    }

    public static float LoadMusicVolume()
    {
        return PlayerPrefs.GetFloat(MusicVolumeKey, DefaultMusicVolume);
    }

    public static void SaveMusicMute(bool isMuted)
    {
        PlayerPrefs.SetInt(MusicMuteKey, isMuted ? 1 : 0);
    }

    public static bool LoadMusicMute()
    {
        return PlayerPrefs.GetInt(MusicMuteKey, 0) == 1;
    }
    #endregion

    #endregion

    #region Fullscreen Save/Load Function
    public static void SaveFullscreen(bool isFullscreen)
    {
        PlayerPrefs.SetInt(FullscreenKey, isFullscreen ? 1 : 0);
    }

    public static bool LoadFullscreen()
    {
        return PlayerPrefs.GetInt(FullscreenKey, DefaultFullscreen ? 1 :0) == 1;
    }
    #endregion

    #endregion

    #region Reset
    public static void ResetAll()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
    #endregion

    #region TODO: Next Iteration
    //TODO: Save level completion state
    //TODO: Load level completion state
    //TODO: Save challenge completion
    //TODO: Load challenge completion
    #endregion


}
