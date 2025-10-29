using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1_ChallengeTracker : MonoBehaviour
{

    #region variables

    [Header("Challenge References")]
    [Tooltip("Parent holding coins.")]
    [SerializeField] private Transform _pawTokenParent;
    [Tooltip("Put all enemies being tracked here")]
    [SerializeField] private EnemyController[] _slimes;
    [Tooltip("Reference to the collider holding the level 1 end trigger.")]
    [SerializeField] private Level1EndTrigger _level1EndTrigger;

    private int totalTokens;
    private int totalSlimes;
    private int collectedTokens = 0;
    private int defeatedSlimes = 0;

    private bool coinsChallengeComplete;
    private bool timeChallengeComplete;
    private bool catsChallengeComplete;
    private bool canTrackChallenges = false;

    #endregion

    #region Start
    private void Start()
    {
        //only track challenges after the first level has been completed
        if (!SaveManager.LoadLevelComplete(1))
        {
            Debug.Log("Level 1 not yet completed - challenge tracking disabled.");
            return;
        }

        canTrackChallenges = true;

        if (_pawTokenParent != null)
        {
            totalTokens = _pawTokenParent.childCount;
        }
        
        totalSlimes = _slimes.Length;

        //subscribe to global events
        CurrencyManager.OnTokenCollected += OnTokenCollected;
        EnemyController.OnEnemyDied += OnEnemyDied;
        Level1EndTrigger.OnLevelCompleted += OnLevelCompleted;
    }
    #endregion

    #region OnDestroy
    private void OnDestroy()
    {
        //unsubcribe to prevent memory leaks
        CurrencyManager.OnTokenCollected -= OnTokenCollected;
        EnemyController.OnEnemyDied -= OnEnemyDied;
        Level1EndTrigger.OnLevelCompleted -= OnLevelCompleted;
    }
    #endregion

    #region Method/Functions

    //count all slimes in nested gameobjects hiearchy
    private int CountSlimes(Transform parent)
    {
        int count = 0;
        foreach (Transform child in parent)
        {
            if (child.GetComponentInChildren<EnemyController>() != null)
            {
                count++;
            }

            //check gain, due to nesting
            count += CountSlimes(child);
        }

        return count;
    }

    private void OnTokenCollected()
    {
        if (!canTrackChallenges) return;

        collectedTokens++;

        //ensure it only triggers when all coins are collected
        if (totalTokens <= 0) return;

        if (!coinsChallengeComplete && collectedTokens >= totalTokens)
        {
            coinsChallengeComplete = true;
            SaveManager.SaveChallengeComplete(1, 1);
            PlayerPrefs.Save();
            ChallengeManager.instance?.MarkChallengeComplete(1, 1);
        }
    }

    public void OnLevelCompleted()
    {
        if (!canTrackChallenges) return;

        if (!timeChallengeComplete && TimeManager.instance.elapsedTime <= 120f)
        {
            timeChallengeComplete = true;
            SaveManager.SaveChallengeComplete(1, 2);
            PlayerPrefs.Save();
            ChallengeManager.instance?.MarkChallengeComplete(1, 2);
        }
    }

    public void OnEnemyDied()
    {
        if (!canTrackChallenges) return;

        defeatedSlimes++;

        if (!catsChallengeComplete && defeatedSlimes >= totalSlimes)
        {
            catsChallengeComplete = true;
            SaveManager.SaveChallengeComplete(1, 3);
            PlayerPrefs.Save();
            ChallengeManager.instance?.MarkChallengeComplete(1, 3);
        }
    }

    #endregion
}
