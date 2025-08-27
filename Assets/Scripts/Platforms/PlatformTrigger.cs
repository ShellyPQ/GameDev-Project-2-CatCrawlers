using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class PlatformTrigger : MonoBehaviour
{
    #region Variables
    [Header("References")]
    [Tooltip("The platform this button will trigger")]
    [SerializeField] GameObject _assignedPlatform;

    private AnimatorController _ani;
    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //change the tag on the assigned platform to Untagged
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("This is the player");
            //The platform will move once it is no longer tagged as a "TriggerPlatform"
            _assignedPlatform.gameObject.tag = "Untagged";
        }
        // trigger animation

        //Unlock SFX?
    }
}
