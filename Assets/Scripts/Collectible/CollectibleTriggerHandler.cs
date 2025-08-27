using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleTriggerHandler : MonoBehaviour
{
    #region Variables
    //reference to our collectable manager
    private CollectibleManager _collectableManager;
    #endregion

    #region Awake
    private void Awake()
    {
        _collectableManager = GetComponent<CollectibleManager>();
    }
    #endregion

    #region Method/Functions
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //collect object
            _collectableManager.Collect(collision.gameObject);

            //destroy object
            Destroy(gameObject);
        }
    }
    #endregion
}
