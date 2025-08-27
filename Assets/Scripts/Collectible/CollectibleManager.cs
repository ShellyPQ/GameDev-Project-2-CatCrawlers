using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Required Components
//This will check if the object holding this script has these components
//If not it will add them
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(CollectibleTriggerHandler))]
#endregion

public class CollectibleManager : MonoBehaviour
{
    #region Variables
    [SerializeField] private CollectibleSOData _collectable;
    #endregion

    #region Reset
    private void Reset()
    {
        //ensure the collider is a trigger
        GetComponent<CircleCollider2D>().isTrigger = true;
        //ensure the radius of this collider is set
        GetComponent<CircleCollider2D>().radius = 1;
    }
    #endregion

    public void Collect(GameObject objectCollected)
    {
        _collectable.Collect(objectCollected);
    }
}
