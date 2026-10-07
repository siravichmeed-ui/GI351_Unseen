using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Raw Score")]
    [SerializeField] private int score = 0;

    [Header("Survival Time")]
    [SerializeField] private float survivalTime = 0f;

    [Header("Final Score")]
    [SerializeField] private int timeScoreMultiplier = 10;

    [Header("Enemies")]
    [SerializeField] private int enemiesDefeated = 0;

    public int Score => score;

    public float SurvivalTime =>
        survivalTime;

    public int EnemiesDefeated =>
        enemiesDefeated;

    // =====================================================
    // TIME BONUS
    // =====================================================

    public int TimeBonus
    {
        get
        {
            return Mathf.FloorToInt(survivalTime) *
                   timeScoreMultiplier;
        }
    }

    // =====================================================
    // FINAL SCORE
    // =====================================================

    public int FinalScore
    {
        get
        {
            return score + TimeBonus;
        }
    }

    private bool isGameOver = false;

    // =====================================================
    // AWAKE
    // =====================================================

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

        survivalTime = 0f;
        isGameOver = false;
    }

    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        if (isGameOver)
            return;

        survivalTime += Time.deltaTime;
    }

    // =====================================================
    // ADD RAW SCORE
    // =====================================================

    public void AddScore(int amount)
    {
        if (amount <= 0)
            return;

        score += amount;

        Debug.Log(
            "Raw Score: " +
            score
        );
    }

    // =====================================================
    // ENEMY DEFEATED
    // =====================================================

    public void AddEnemyDefeated()
    {
        enemiesDefeated++;

        Debug.Log(
            "Enemies Defeated: " +
            enemiesDefeated
        );
    }

    // =====================================================
    // STOP SURVIVAL TIME
    // =====================================================

    public void StopSurvivalTime()
    {
        isGameOver = true;
    }

    // =====================================================
    // RESET
    // =====================================================

    public void ResetScore()
    {
        score = 0;
        enemiesDefeated = 0;
        survivalTime = 0f;
        isGameOver = false;
    }
}