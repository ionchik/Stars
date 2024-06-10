using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    public void StartNewGame()
    {
        SceneManager.LoadScene("GamePlay");
    }

    public void OpenMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
