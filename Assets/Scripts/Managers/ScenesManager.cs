using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    //Make this class a singleton
    public static ScenesManager Instance;

    #region Awake
    private void Awake()
    {
        Instance = this;
    }
    #endregion

    #region Enums
    public enum Scene
    {
        //Add to this enum as new scenes are added (Ensure naming here match the scene names)
        MainMenu,
        Test_Scene
    }
    #endregion

    #region Function/Methods
    //Load in a specified scene
    public void LoadScene(Scene scene)
    {
        SceneManager.LoadScene(scene.ToString());
    }

    //Load in the scene that will start a new game
    public void LoadNewGame()
    {
        SceneManager.LoadScene(Scene.Test_Scene.ToString());
    }

    //Load the next scene (when a level is complete)
    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    //Go back to the main menu when this function/method is called
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(Scene.MainMenu.ToString());
    }

    //Quit the game and or playmode (in unity) when this function/method is called
    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    #endregion
}
