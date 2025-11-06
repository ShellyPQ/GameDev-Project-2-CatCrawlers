using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableMovingPlatform : MonoBehaviour
{
    #region Variables

    [Header("References")]
    [SerializeField] GameObject _assignedPlatform;

    #endregion
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //The platform will move once it is no longer tagged as a "TriggerPlatform"
            _assignedPlatform.gameObject.tag = "Untagged";
        }
    }
}
