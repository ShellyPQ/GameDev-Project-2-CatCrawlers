using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    //singleton
    public static TimeManager instance;

    #region Variables

    [Header("Timer Properties")]
    public float elapsedTime = 0f;
    public bool isRunning = false;
    public bool timerVisible = true;

    [Header("References")]
    [SerializeField] private TextMeshProUGUI _timerText;

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
        StartTimer();    
    }
    #endregion

    #region Update
    private void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimer();
        }
    }

    #endregion

    #region Method/Functions
    public void StartTimer()
    {
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResetTimer()
    {
        elapsedTime = 0f;
        UpdateTimer();
    }

    public void EnableTimer(bool enable)
    {
        timerVisible = enable;

        if (_timerText != null)
        {
            _timerText.enabled = enable;
        }
    }

    private void UpdateTimer()
    {
        if (_timerText == null || !timerVisible)
        {
            return;
        }

        int min = Mathf.FloorToInt(elapsedTime / 60);
        int sec = Mathf.FloorToInt(elapsedTime % 60);

        _timerText.text = $"{min:00}:{sec:00}";
    }
    #endregion
}
