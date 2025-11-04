using UnityEngine;

[CreateAssetMenu(menuName = "Collectible/Catnip", fileName = "New Catnip Collectible")]
public class CollectibleCatnipSO : CollectibleSOData
{
    #region Variables
    [Header("Catnip Settings")]
    public float frenzyDuration = 5f;
    public float frenzySpeed = 7f;
    #endregion

    #region Mathod/Functions
    public override void Collect(GameObject objectCollected)
    {
        CatnipFrenzyBuff frenzy  = objectCollected.GetComponent<CatnipFrenzyBuff>();
        if (frenzy != null)
        {
            frenzy.StartFrenzy(frenzyDuration, frenzySpeed);
        }
    }

    #endregion
}
