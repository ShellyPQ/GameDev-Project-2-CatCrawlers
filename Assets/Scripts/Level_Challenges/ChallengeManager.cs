using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;

public class ChallengeManager : MonoBehaviour
{
    //singleton
    public static ChallengeManager instance { get; private set; }

    #region Variables

    [Header("Challenge Panel References")]
    [Tooltip("Challenges panel gameobject")]
    [SerializeField] private GameObject _challengesPanel;
    [Tooltip("Paw Markers used to mark a complete challenge.")]
    [SerializeField] private GameObject[] _pawMarkersComplete;
    [Tooltip("Texts for each challenge.")]
    [SerializeField] private TextMeshProUGUI[] _challengeTexts;

    [Header("Level Challenge Texts.")]
    [TextArea] public string[] level1Challenges;
    [TextArea] public string[] level2Challenges;
    [TextArea] public string[] level3Challenges;


    private int _currentlevel = 1;
    #endregion

    #region Awake
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    #endregion

    #region Start
    private void Start()
    {
        //hide panel at start - panel only displays when hovering over level buttons
        _challengesPanel.SetActive(false);
    }
    #endregion

    #region Method/Functions

    //function called when hovering over a level button
    public void DisplayChallenge(int level)
    {
        //only if level 1 has been completed
        if (!SaveManager.LoadLevelComplete(1)) return;

        //activate panel
        _challengesPanel.SetActive(true);

        _currentlevel = level;
        string[] selectedChallenges = null;

        switch (level)
        {
            case 1:
                selectedChallenges = level1Challenges;
                break;
            case 2:
                selectedChallenges = level2Challenges;
                break;
            case 3:
                selectedChallenges = level3Challenges;
                break;
            default:
                selectedChallenges = null;
                break;
        }

        if (selectedChallenges == null) return;

        //update challenge text
        for (int i = 0; i < _challengeTexts.Length; i++)
        {
            _challengeTexts[i].text = i < selectedChallenges.Length ? selectedChallenges[i] : "";
        }

        //toggle complete paw
        UpdatePawMarker();
        //apply text strikethrough
        ApplyStrikethroughs();

    }

    public void MarkChallengeComplete(int level, int challengeIndex)
    {
        if (level != _currentlevel) return;

        int index = challengeIndex - 1;
        if (index < 0 || index >= _challengeTexts.Length) return;

        //enable paw icon
        if (index < _pawMarkersComplete.Length && _pawMarkersComplete[index] != null)
        {
            _pawMarkersComplete[index].SetActive(true);
        }
    }

    public void HideChallenges()
    {
        _challengesPanel.SetActive(false);
    }

    //function used to toggle the complete paw icons on/off based on saved challenge completion
    private void UpdatePawMarker()
    {
        for (int i = 0; i < _pawMarkersComplete.Length; i++)
        {
            bool isComplete = SaveManager.LoadChallengeComplete(_currentlevel, i + 1);

            //toggle paw visibility
            if (_pawMarkersComplete[i] != null)
            {
                _pawMarkersComplete[i].SetActive(isComplete);
            }
        }
    }

    //function that strikethrough the challenge text when completed for more player feedback
    private void ApplyStrikethroughs()
    {
        for (int i = 0; i < _challengeTexts.Length; i++)
        {
            bool isComplete = SaveManager.LoadChallengeComplete(_currentlevel, i + 1);

            if (_challengeTexts[i] != null)
            {
                if (isComplete)
                {
                    //apply strikethrough to text
                    _challengeTexts[i].fontStyle |= TMPro.FontStyles.Strikethrough;
                }
                else
                {

                    //apply strikethrough to text
                    _challengeTexts[i].fontStyle &= ~TMPro.FontStyles.Strikethrough;
                }
            }
        }
    }
    #endregion
}