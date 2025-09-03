using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    #region Variables
    [Header("SFX Clips")]
    public AudioClip jump;
    public AudioClip land;
    public AudioClip playerHurt;
    public AudioClip melee;
    public AudioClip yarnStun;
    public AudioClip enemyHurt;
    public AudioClip pawToken;
    public AudioClip yarnBall;

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
    public void playSFX(string sfxName)
    {
        //data that will be called in other scripts to player the desired audio
        switch (sfxName)
        {
            case "jump":
                SoundObjectCreation(jump);
                break;
            case "land":
                SoundObjectCreation(land);
                break;
            case "playerHurt":
                SoundObjectCreation(playerHurt);
                break;
            case "melee":
                SoundObjectCreation(melee);
                break;
            case "yarnStun":
                SoundObjectCreation(yarnStun);
                break;
            case "enemyHurt":
                SoundObjectCreation(enemyHurt);
                break;
            case "pawToken":
                SoundObjectCreation(pawToken);
                break;
            case "yarnBall":
                SoundObjectCreation(yarnBall);
                break;
            default:
                break;
        }
    }
    private void SoundObjectCreation(AudioClip clip)
    {
        //create SoundObject gameobject
        GameObject newObject = Instantiate(soundObject, transform);
        //assign audioclip to audio source
        newObject.GetComponent<AudioSource>().clip = clip;
        //place the audio
        newObject.GetComponent<AudioSource>().Play();
    }
    #endregion
}
