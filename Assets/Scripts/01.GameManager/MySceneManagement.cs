using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManagement : MonoBehaviour 
{
    public static int CurrentFloor = 1;
    private void Start()
    {
        //한윤구 추가
        string sceneName = SceneManager.GetActiveScene().name;
        Debug.Log("Scene loaded: " + sceneName);

        CurrentFloor = ExtractFloorNumber(sceneName);
        Debug.Log($"[MySceneManagement] Current floor set to {CurrentFloor}");
        
        Debug.Log("Scene Manager is alive in scene: " + SceneManager.GetActiveScene().name);
    }
    //한윤구 추가
    private int ExtractFloorNumber(string sceneName)
    {
        if (!sceneName.Contains("Floor"))
            return 0;

        string numberPart = sceneName.Replace("stFloor", "")
                                     .Replace("ndFloor", "")
                                     .Replace("rdFloor", "")
                                     .Replace("thFloor", "");
        if (int.TryParse(numberPart, out int floor))
            return floor;

        return 0;
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
