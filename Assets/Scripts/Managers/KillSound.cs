using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillSound : MonoBehaviour
{
    #region Variables
    //variable to access audio source script
    public AudioSource source;
    #endregion

    #region Update
    private void Update()
    {
        //check to see if audio is playing from script
        if (!source.isPlaying)
        {
            //destroy object after audio plays
            Destroy(gameObject);
        }
    }
    #endregion
}
