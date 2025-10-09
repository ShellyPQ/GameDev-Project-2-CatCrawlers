using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Collectible/YarnBit", fileName = "Yarn Bit Data")]
public class CollectibleYarnBitSO : CollectibleSOData
{
    #region Variables
    [Header("Collectible Data")]
    [Tooltip("Current Yarn Bit Count")]
    public int currentYarnAmount = 1;

    [Header("Respawn Settings")]
    public float respawnTime = 5f; 
    #endregion

    public override void Collect(GameObject objectCollected)
    {
        RangedAttack ranged = objectCollected.GetComponent<RangedAttack>();
        if (ranged != null)
        {
            bool added = ranged.AddAmmo(currentYarnAmount);

            if (added)
            {
                // Update the HUD for ammo
                HUDManager.instance.UpdateAmmoText(ranged.GetAmmo());
            }
        }
    }
}
