using UnityEngine;
using UnityEngine.SceneManagement;
public class Scene : MonoBehaviour
{
    public void GoToGame()
    {
        SceneManager.LoadScene(1);
    }
    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void GameStart()
    {
        SceneManager.LoadScene(2);
    }
}