using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Collectible/YarnBit", fileName = "Yarn Bit Data")]
public class CollectibleYarnBitSO : CollectibleSOData
{
    #region Variables
    [Header("Collectible Data")]
    [Tooltip("Current Yarn Bit Count")]
    public int currentYarnBitCount = 1;

    #endregion

    public override void Collect(GameObject objectCollected)
    {
        //Call instance that will increment everytime the player collects a yarn bit
    }
}
