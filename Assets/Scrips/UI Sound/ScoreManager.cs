using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Score")]
    [SerializeField] private int score = 0;

    [Header("Survival Time")]
    [SerializeField] private float survivalTime = 0f;

    [Header("Enemies")]
    [SerializeField] private int enemiesDefeated = 0;
    public int Score => score;

    public float SurvivalTime =>
        survivalTime;

    public int EnemiesDefeated =>
       enemiesDefeated;

    private bool isGameOver = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        survivalTime = 0f;
        isGameOver = false;
    }
    private void Update()
    {
        if (isGameOver)
            return;

        survivalTime += Time.deltaTime;
    }
    public void AddScore(int amount)
    {
        if (amount <= 0)
            return;

        score += amount;

        Debug.Log("Score: " + score);
    }

    public void ResetScore()
    {
        score = 0;
        enemiesDefeated = 0;
        survivalTime = 0f;
        isGameOver = false;
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
    // SURVIVAL TIME
    // =====================================================

    public void StopSurvivalTime()
    {
        isGameOver = true;
    }




}