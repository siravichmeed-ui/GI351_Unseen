using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("Spawn Point")]
    [SerializeField] private Transform spawnPoint;

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


        if (spawnPoint == null)
        {
            Debug.LogWarning(
                "ยังไม่ได้ใส่ Spawn Point"
            );

            return;
        }


        // สุ่ม Enemy
        int randomIndex =
            Random.Range(
                0,
                enemyPrefabs.Length
            );


        GameObject selectedEnemy =
            enemyPrefabs[randomIndex];


        // สร้าง Enemy
        Instantiate(
            selectedEnemy,
            spawnPoint.position,
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