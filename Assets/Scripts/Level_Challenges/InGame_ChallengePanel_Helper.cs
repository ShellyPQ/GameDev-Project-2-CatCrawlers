using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGame_ChallengePanel_Helper : MonoBehaviour
{
    #region Variables
    [SerializeField] private int levelNumber = 1;
    [SerializeField] private GameObject _challengePanel;
    [SerializeField] private GameObject _timerText;
    #endregion

    #region Start
    private void Start()
    {
        // Check immediately when level starts
        CheckUnlockState();
    }
    #endregion

    #region OnEnable
    private void OnEnable()
    {
        // Check again when pause panel (or parent) re-enables
        CheckUnlockState();
    }
    #endregion

    #region Method/Functions
    private void CheckUnlockState()
    {
        bool level1Complete = SaveManager.LoadLevelComplete(1);

        if (_challengePanel == null) return;

        if (level1Complete)
        {
            _challengePanel.SetActive(true);
            _timerText?.SetActive(true);
            ChallengeManager.instance?.DisplayChallenge(levelNumber);
        }
        else
        {
            _challengePanel.SetActive(false);
            _timerText?.SetActive(false);
        }
    }
    #endregion
}