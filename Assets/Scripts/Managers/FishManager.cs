using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishManager : MonoBehaviour
{
   public static FishManager instance;

    #region Variables

    [Header("Fish Tracking Properties")]
    public int currentFishCount = 0;
    public int totalFishInLevel = 5;
    #endregion

    #region Awake
    private void Awake()
    {
        instance = this;
    }
    #endregion

    #region Method/Functions
    public void IncreaseFishCount(int value)
    {
        currentFishCount += value;

        //update UI
    }
    #endregion
}
