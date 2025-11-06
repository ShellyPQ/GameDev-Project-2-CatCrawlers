using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillSound : MonoBehaviour
{
    #region Variables
    //variable to access audio source script
    public AudioSource source;
    #endregion

    #region Start
    private void Start()
    {
        if (source == null)
        {
            source = GetComponent<AudioSource>();
        }

        //start delayed checl
        StartCoroutine(KillSoundWhenDown());
    }
    #endregion

    private IEnumerator KillSoundWhenDown()
    {
        //when two frames
        yield return null;
        yield return null;

        //wait until sound finishes
        yield return new WaitWhile(() => source.isPlaying);

        //destroy the object after playback
        Destroy(gameObject);
    }
}
