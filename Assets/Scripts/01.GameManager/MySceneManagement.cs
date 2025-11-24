using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManagement : MonoBehaviour 
{
    private void Start()
    {
        Debug.Log("Scene Manager is alive in scene: " + SceneManager.GetActiveScene().name);
    }
    public void loadNextLevel()
    {
        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextScene);
    }

    public void reloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
    }

    public void loadMainMenu()
    {
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1;
    }

    #region Cheat
    public void WinGame()
    {
        return;
    }
    #endregion
}
