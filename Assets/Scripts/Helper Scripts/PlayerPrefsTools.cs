using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerPrefsTools : MonoBehaviour
{

#if UNITY_EDITOR
    [MenuItem("Tools/Clear PlayerPrefs")]

    public static void ClearPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log(" PlayePrefs Cleared! :)");
    }
#endif

    #region Update
    private void Update()
    {
        //hold ctrl + shift + r
        if (((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKey(KeyCode.R)))
        {
            SaveManager.ResetAll();
            Debug.Log(" PlayePrefs Cleared! :)");
        }
    }
    #endregion
}