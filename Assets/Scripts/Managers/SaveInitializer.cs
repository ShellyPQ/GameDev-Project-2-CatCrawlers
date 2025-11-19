using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveInitializer : MonoBehaviour
{
    //increment this when you change save structure
    private const int SaveVersion = 1;

    private void Awake()
    {    
        if (!PlayerPrefs.HasKey("initialized"))
        {
            int currentVersion = PlayerPrefs.GetInt("save_version", -1);

            Debug.Log("First run detected. Resetting all PlayerPrefs...");
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetInt("initialized", SaveVersion);
            PlayerPrefs.Save();
        }
    }
}
