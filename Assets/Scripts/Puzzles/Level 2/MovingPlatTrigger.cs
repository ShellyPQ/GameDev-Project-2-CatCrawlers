using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatTrigger : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The platform this button will trigger")]
    [SerializeField] GameObject _assignedPlatform;

    #region Method/Functions
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //change the tag on the assigned platform to Untagged
        if (collision.gameObject.tag == "Player")
        {
            //The platform will move once it is no longer tagged as a "TriggerPlatform"
            _assignedPlatform.gameObject.tag = "Untagged";
        }
        //Unlock SFX?
    }
    #endregion
}
