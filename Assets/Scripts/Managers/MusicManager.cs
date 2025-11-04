using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    //Singleton
    public static MusicManager instance;

    #region Awake
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            //make sure there is only one
            Destroy(instance);
        }
    }
    #endregion

    #region Method/Functions

    public void PlayMusic(AudioClip clip, float fadeTime = 0.5f)
    {
        StartCoroutine(FadeMusic(clip, fadeTime));  
    }

    #endregion

    #region Coroutine

    private IEnumerator FadeMusic(AudioClip newClip, float fadeTime)
    {
        AudioSource audio = GetComponent<AudioSource>();

        float startVolume = audio.volume;

        //fadeout
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            audio.volume = Mathf.Lerp(startVolume, 0, t / fadeTime);
            yield return null;
        }

        audio.clip = newClip;
        audio.Play();

        //fade in
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            audio.volume = Mathf.Lerp(0, startVolume, t / fadeTime);
            yield return null;
        }

        audio.volume = startVolume;
    }

    #endregion
}
