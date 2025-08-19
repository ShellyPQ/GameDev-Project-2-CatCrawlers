using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingAudio : MonoBehaviour
{

    [SerializeField] private AudioClip _testAudioClip;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision Detected");
        if (collision.gameObject.tag == ("Player"))
        {
            AudioSpawner.instance.PlaySoundClip(_testAudioClip, collision.transform, 1f);
        }
    }
}
