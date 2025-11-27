using UnityEngine;

public class ResetGameBtn : MonoBehaviour
{
    public void ResetGame()
    {
        GameManager.Instance.ResetGame();
    }
}
