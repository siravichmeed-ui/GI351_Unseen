using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [Header("Score")]
    [SerializeField] private TMP_Text scoreText;

    [Header("Time")]
    [SerializeField] private TMP_Text timeText;

    private void Update()
    {
        if (ScoreManager.Instance == null)
            return;

        // Score
        if (scoreText != null)
        {
            scoreText.text =
                ScoreManager.Instance.Score.ToString();
        }

        // Survival Time
        if (timeText != null)
        {
            float time =
                ScoreManager.Instance.SurvivalTime;

            int minutes =
                Mathf.FloorToInt(time / 60f);

            int seconds =
                Mathf.FloorToInt(time % 60f);

            timeText.text =
                string.Format(
                    "{0:00}:{1:00}",
                    minutes,
                    seconds
                );
        }
    }
}