using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("Result")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text survivalTimeText;
    [SerializeField] private TMP_Text enemiesDefeatedText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        Time.timeScale = 1f;
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // Score
        if (scoreText != null &&
            ScoreManager.Instance != null)
        {
            scoreText.text =
                ScoreManager.Instance.Score.ToString();
        }

        // Survival Time
        if (survivalTimeText != null &&
            ScoreManager.Instance != null)
        {
            survivalTimeText.text =
                FormatTime(
                    ScoreManager.Instance.SurvivalTime
                );
        }

        // Enemies Defeated
        if (enemiesDefeatedText != null &&
            ScoreManager.Instance != null)
        {
            enemiesDefeatedText.text =
                ScoreManager.Instance.EnemiesDefeated.ToString();
        }

        // หยุดการนับเวลา
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.StopSurvivalTime();
        }

        Time.timeScale = 0f;
    }
    private string FormatTime(float time)
    {
        int minutes =
            Mathf.FloorToInt(time / 60f);

        int seconds =
            Mathf.FloorToInt(time % 60f);

        return string.Format(
            "{0:00}:{1:00}",
            minutes,
            seconds
        );
    }
    // QUIT
    public void Quit()
    {
        Debug.Log("QUIT BUTTON WORK");
        Time.timeScale = 1f;

        SceneManager.LoadScene(0);
    }
    public void Restart()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}