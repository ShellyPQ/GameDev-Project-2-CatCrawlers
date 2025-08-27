using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    //Singleton
    public static CurrencyManager instance;

    #region Variables
    [Header("Properties")]
    [Tooltip("How many paw tokens does the player have")]
    public int _currentPawTokenAmount = 0;
    #endregion

    #region Awake
    private void Awake()
    {
       if (instance == null)
        {
            instance = this;
        }
       else
        {
            //make sure there is only one
            Destroy(gameObject);
        }
    }
    #endregion

    #region Method/Function
    public void IncremenentTokenCurrency(int Amount)
    {
        //update the current currency amount
        _currentPawTokenAmount += Amount;

        //Update text display when this function is called
        HUDManager.instance.UpdateCurrencyText(_currentPawTokenAmount);
    }
    #endregion
}
