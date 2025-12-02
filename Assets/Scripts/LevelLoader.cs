using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoaderButton : MonoBehaviour
{
    public string levelName; // Which json to load

    public string gameSceneName = "Game Scene"; // The scene to load

    public void LoadLevel()
    {
        PlayerPrefs.SetString("TargetLevel", levelName); // Save level name for next scene
        PlayerPrefs.Save();

        SceneManager.LoadScene(gameSceneName); // Load gameplay scene
    }
}
