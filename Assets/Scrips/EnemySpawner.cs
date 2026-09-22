using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private int maxEnemies = 5;

    private float nextSpawnTime;


    private void Update()
    {
        // ถ้า Enemy ถึงจำนวนสูงสุดแล้ว
        if (GetCurrentEnemyCount() >= maxEnemies)
            return;


        // ถึงเวลาสร้าง Enemy หรือยัง
        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();

            nextSpawnTime =
                Time.time + spawnInterval;
        }
    }


    // =====================================================
    // SPAWN ENEMY
    // =====================================================

    private void SpawnEnemy()
    {
        if (enemyPrefabs == null ||
            enemyPrefabs.Length == 0)
        {
            Debug.LogWarning(
                "ยังไม่ได้ใส่ Enemy Prefab"
            );

            return;
        }


        if (spawnPoints == null ||
            spawnPoints.Length == 0)
        {
            Debug.LogWarning(
                "ยังไม่ได้ใส่ Spawn Point"
            );

            return;
        }


        // สุ่ม Enemy
        int randomEnemyIndex =
            Random.Range(
                0,
                enemyPrefabs.Length
            );

        GameObject selectedEnemy =
            enemyPrefabs[randomEnemyIndex];


        // สุ่มจุด Spawn
        int randomSpawnIndex =
            Random.Range(
                0,
                spawnPoints.Length
            );

        Transform selectedSpawnPoint =
            spawnPoints[randomSpawnIndex];

        if (selectedSpawnPoint == null)
        {
            Debug.LogWarning(
                "Spawn Point index " + randomSpawnIndex + " ยังไม่ได้ใส่ Transform"
            );

            return;
        }


        // สร้าง Enemy
        Instantiate(
            selectedEnemy,
            selectedSpawnPoint.position,
            Quaternion.identity
        );
    }


    // =====================================================
    // COUNT ENEMIES
    // =====================================================

    private int GetCurrentEnemyCount()
    {
        Enemy[] enemies =
            FindObjectsByType<Enemy>(
                FindObjectsSortMode.None
            );

        return enemies.Length;
    }
}