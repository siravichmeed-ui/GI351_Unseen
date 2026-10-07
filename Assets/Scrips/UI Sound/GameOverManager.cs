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
    [SerializeField] private TMP_Text finalScoreText;
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

    // =====================================================
    // GAME OVER
    // =====================================================

    public void ShowGameOver()
    {
        if (ScoreManager.Instance == null)
            return;

        // =================================================
        // STOP SCORE TIME
        // =================================================

        ScoreManager.Instance.StopSurvivalTime();

        // =================================================
        // SHOW PANEL
        // =================================================

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // =================================================
        // RAW SCORE
        // =================================================

        if (scoreText != null)
        {
            scoreText.text =
                ScoreManager.Instance.Score.ToString();
        }

        // =================================================
        // FINAL SCORE
        // =================================================

        if (finalScoreText != null)
        {
            finalScoreText.text =
                ScoreManager.Instance.FinalScore.ToString();
        }

        // =================================================
        // SURVIVAL TIME
        // =================================================

        if (survivalTimeText != null)
        {
            survivalTimeText.text =
                FormatTime(
                    ScoreManager.Instance.SurvivalTime
                );
        }

        // =================================================
        // ENEMIES DEFEATED
        // =================================================

        if (enemiesDefeatedText != null)
        {
            enemiesDefeatedText.text =
                ScoreManager.Instance.EnemiesDefeated.ToString();
        }

        // =================================================
        // STOP GAME
        // =================================================

        Time.timeScale = 0f;
    }

    // =====================================================
    // FORMAT TIME
    // =====================================================

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

    // =====================================================
    // QUIT
    // =====================================================

    public void Quit()
    {
        Debug.Log("QUIT BUTTON WORK");

        Time.timeScale = 1f;

        SceneManager.LoadScene(0);
    }

    // =====================================================
    // RESTART
    // =====================================================

    public void Restart()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}