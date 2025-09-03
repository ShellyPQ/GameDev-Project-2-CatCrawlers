using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    //Singleton
    public static HUDManager instance;

    #region Variables
    [Header("References")]
    [Tooltip("Text that will display how many paw tokens the player has collected")]
    [SerializeField] private TextMeshProUGUI _currentPawTokensText;
    [SerializeField] private TextMeshProUGUI _remainingLives;
    [SerializeField] private TextMeshProUGUI _currentAmmoText;
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
            Destroy(instance);
        }
    }
    #endregion

    #region Method/Function
    public void UpdateCurrencyText(int currentTokens)
    {
        //Update the text to display the current amount of tokens collected
        _currentPawTokensText.text = currentTokens.ToString();
    }

    public void UpdateRemainingLivesText(int currentLives)
    {
        _remainingLives.text = currentLives.ToString();
    }
    public void UpdateAmmoText(int currentAmmo) 
    {
        if (_currentAmmoText != null)
        {
            _currentAmmoText.text = currentAmmo.ToString();
        }            
    }

    #endregion
}
