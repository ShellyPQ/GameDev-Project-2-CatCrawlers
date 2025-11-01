using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Collectible/Fish", fileName = "New Fish Collectible")]
public class CollectibleFishSO : CollectibleSOData
{
    #region Variables

    [Header("Fish Collectible")]
    [Tooltip("How many fish this collectible grants when collected")]
    public int fishValue = 1;

    #endregion

    public override void Collect(GameObject objectCollected)
    {
        FishManager.instance.IncreaseFishCount(fishValue);

        //notify challenge track 

        //notify UI
    }
}
