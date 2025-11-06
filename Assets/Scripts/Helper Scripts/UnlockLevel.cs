using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnlockLevel : MonoBehaviour
{
    #region Variables
    public int levelIndex;
    private Button _levelSelectButton;
    #endregion

    #region Awake
    private void Awake()
    {
        _levelSelectButton = GetComponent<Button>();
    }
    #endregion

    #region OnEnable
    private void OnEnable()
    {
        bool unlocked = SaveManager.IsLevelUnlocked(levelIndex);
        _levelSelectButton.interactable = unlocked;
    }
    #endregion
}
