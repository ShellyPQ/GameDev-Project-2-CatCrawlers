using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSpawner : MonoBehaviour
{
    public static AudioSpawner instance;

    [SerializeField] private AudioSource audioSourceObject;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void PlaySoundClip(AudioClip audioClip, Transform spawnLocation, float volume)
    {
        //spawn in gameobject
        AudioSource audioSource = Instantiate(audioSourceObject, spawnLocation.position, Quaternion.identity);
        //assign the audioclip
        audioSource.clip = audioClip;
        //assign volume
        audioSource.volume = volume;
        //play sound
        audioSource.Play();
        //get length
        float clipLength = audioSource.clip.length;
        //destroy the clip after it is done playing
        Destroy(audioSource.gameObject, clipLength);
    }
}
