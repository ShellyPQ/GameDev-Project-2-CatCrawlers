using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Collectible/Currency", fileName = "New Token Collectible")]

public class CollectibleCurrencySO : CollectibleSOData
{
    #region Variables
    [Header("Collectible Data")]
    [Tooltip("Current Token Count")]
    public int currentTokenCount = 1;

    #endregion
    public override void Collect(GameObject objectCollected)
    {
        CurrencyManager.instance.IncremenentTokenCurrency(currentTokenCount);
    }
}
