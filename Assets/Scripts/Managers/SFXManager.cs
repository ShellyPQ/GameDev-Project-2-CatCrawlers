using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    #region Variables
    [Header("Mixer Group")]
    [SerializeField] private AudioMixerGroup _sfxMixerGroup;

    [Header("SFX Clips")]
    public AudioClip jump;
    public AudioClip land;
    public AudioClip playerHurt;
    public AudioClip meleeSwipe;
    public AudioClip yarnStun;
    public AudioClip enemyHurt;
    public AudioClip pawToken;
    public AudioClip yarnBall;
    public AudioClip buttonPress;
    public AudioClip sfxSettingsButton;
    public AudioClip dummyExplosion;
    public AudioClip doorUnlock;

    //sound object
    public GameObject soundObject;
    #endregion

    #region Awake
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region Method/Behaviours

    //method that holds the switch statement managing the audio clips that can be used
    private AudioClip GetClipName(string sfxName)
    {
        switch (sfxName)
        {
            case "jump":
                return (jump);
            case "land":
                return (land);
            case "playerHurt":
                return (playerHurt);
            case "meleeSwipe":
                return (meleeSwipe);
            case "yarnStun":
                return (yarnStun);
            case "enemyHurt":
                return (enemyHurt);
            case "pawToken":
                return (pawToken);
            case "yarnBall":
                return (yarnBall);
            case "buttonPress":
                return (buttonPress);
            case "sfxSettingsButton":
                return (sfxSettingsButton);
            case "dummyExplosion":
                return (dummyExplosion);
            case "doorUnlock":
                return (doorUnlock);
            default:
                //if no clip is found based on the string name given, return null
                return null;
        }
    }

    //play sfx of string called
    public void playSFX(string sfxName)
    {
        //find the audio clip and store in variable
        AudioClip clip = GetClipName(sfxName);

        //if the clip was found
        if (clip != null)
        {
            //create a sound object and play the clip
            SoundObjectCreation(clip);
        }
    }

    //return the audio clip without playing the sfx
    public AudioClip GetClip(string sfxName)
    {
        //use this function to return what the audio clip is
        return GetClipName(sfxName);
    }
    private void SoundObjectCreation(AudioClip clip)
    {
        //create SoundObject gameobject
        GameObject newObject = Instantiate(soundObject, transform);
        AudioSource source = newObject.GetComponent<AudioSource>();

        //assign the mixergroup before playing
        if (_sfxMixerGroup != null)
        {
            source.outputAudioMixerGroup = _sfxMixerGroup;
        }

        //assign audioclip to audio source
        source.clip = clip;        
        source.Play();
    }
    #endregion
}
