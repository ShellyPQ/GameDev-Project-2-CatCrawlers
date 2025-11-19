using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    #region Variables
    //settings Menu Keys
    private const string BrightnessKey = "brightness";
    private const string MasterVolumeKey = "volume_master";
    private const string SFXVolumeKey = "volume_sfx";
    private const string MusicVolumeKey = "volume_music";
    private const string MasterMuteKey = "mute_master";
    private const string MusicMuteKey = "mute_music";
    private const string SFXMuteKey = "mute_sfx";
    private const string FullscreenKey = "fullscreen";

    //level progress keys
    private const string Level1CompleteKey = "level1_complete";
    private const string Level2CompleteKey = "level2_complete";
    private const string Level3CompleteKey = "level3_complete";

    //settingsdefault values
    private const float DefaultBrightness = 0;    
    private const float DefaultMasterVolume = 0.3f;
    private const float DefaultSFXVolume = 0.1f;
    private const float DefaultMusicVolume = 0.3f;
    private const bool DefaultFullscreen = true;

    #endregion

    #region Settings Menu Save/Load Function/Methods

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

    #region Level Select Menu Save/Load Function/Methods
    private static string GetLevelKey(int level)
    {
        return "level" + level + "_complete";
    }

    public static void SaveLevelComplete(int level)
    {
        PlayerPrefs.SetInt(GetLevelKey(level), 1);
        PlayerPrefs.Save();
    }

    public static bool LoadLevelComplete(int level)
    {
        return PlayerPrefs.GetInt(GetLevelKey(level), 0) == 1;
    }

    #endregion

    #region Challenge Completion Save/Load Fuction/Methods

    private static string GetChallengeKey(int level, int challengeIndex)
    {
        return "challenge" + level + "_" + challengeIndex;
    }

    public static void SaveChallengeComplete(int level, int challengeIndex)
    {
        PlayerPrefs.SetInt(GetChallengeKey(level, challengeIndex), 1);
    }

    public static bool LoadChallengeComplete(int level, int challengeIndex)
    {
        return PlayerPrefs.GetInt(GetChallengeKey(level, challengeIndex), 0) == 1;
    }

    public static bool IsLevelUnlocked(int level)
    {
        //level 1 is always unlocked
        if (level == 1) return true;

        return LoadLevelComplete(level - 1);
     }
    #endregion

    #region Reset
    public static void ResetAll()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
    #endregion
}
