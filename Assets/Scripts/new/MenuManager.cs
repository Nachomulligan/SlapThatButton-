
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string gameSceneName = "GameScene";
    [SerializeField] private string menuSceneName = "MenuScene";

    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void RetryGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}
