using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // =====================================================
    // ENEMY
    // =====================================================

    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject[] enemyPrefabs;


    [Header("Enemy Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;


    [Header("Enemy Spawn Settings")]
    [SerializeField] private float spawnInterval = 2f;

    [SerializeField] private int maxEnemies = 8;


    private float nextSpawnTime;


    // =====================================================
    // ENEMY DENSITY
    // =====================================================

    [Header("Enemy Density")]
    [SerializeField] private Transform player;

    [SerializeField] private float densityRadius = 15f;

    [SerializeField] private int minimumNearbyEnemies = 2;


    // =====================================================
    // GUARANTEED ENEMY
    // =====================================================

    [Header("Guaranteed Enemy")]
    [SerializeField] private int guaranteedSpawnAmount = 2;

    [SerializeField] private float guaranteedSpawnCooldown = 8f;

    [SerializeField] private float minimumSpawnDistance = 8f;

    [SerializeField] private float maximumSpawnDistance = 18f;


    private float nextGuaranteedSpawnTime;


    // =====================================================
    // LIGHT ITEM
    // =====================================================

    [Header("Light Item")]
    [SerializeField] private GameObject lightItemPrefab;


    [Header("Light Item Spawn Points")]
    [SerializeField] private Transform[] itemSpawnPoints;


    [Header("Light Item Spawn Settings")]
    [SerializeField] private int maxLightItems = 3;


    private List<GameObject> spawnedLightItems =
        new List<GameObject>();


    // =====================================================
    // HEALTH POTION DROP
    // =====================================================

    [Header("Health Potion Drop")]
    [SerializeField] private GameObject healthPotionPrefab;


    [SerializeField]
    [Range(0f, 100f)]
    private float healthPotionDropChance = 30f;


    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        // หา Player อัตโนมัติ
        if (player == null)
        {
            PlayerController playerController =
                FindFirstObjectByType<PlayerController>();


            if (playerController != null)
            {
                player =
                    playerController.transform;
            }
        }


        // Spawn Light Item ตอนเริ่มเกม
        SpawnInitialLightItems();


        // ให้ระบบ Guaranteed Enemy
        // เริ่มตรวจหลังจาก 3 วินาที
        nextGuaranteedSpawnTime =
            Time.time + 3f;
    }


    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        // Enemy Spawn ปกติ
        UpdateEnemySpawner();


        // Enemy Density
        UpdateEnemyDensity();


        // Light Item Spawn
        UpdateLightItemSpawner();
    }


    // =====================================================
    // NORMAL ENEMY SPAWNER
    // =====================================================

    private void UpdateEnemySpawner()
    {
        // ถ้า Enemy ถึงจำนวนสูงสุดแล้ว
        if (
            GetCurrentEnemyCount() >=
            maxEnemies
        )
        {
            return;
        }


        // ถึงเวลาสร้าง Enemy หรือยัง
        if (
            Time.time >=
            nextSpawnTime
        )
        {
            SpawnEnemy();


            nextSpawnTime =
                Time.time +
                spawnInterval;
        }
    }


    // =====================================================
    // ENEMY DENSITY
    // =====================================================

    private void UpdateEnemyDensity()
    {
        // ไม่มี Player
        if (player == null)
        {
            return;
        }


        // จำนวน Enemy ทั้งหมด
        int currentEnemyCount =
            GetCurrentEnemyCount();


        // ถ้า Enemy เต็มแล้ว
        if (
            currentEnemyCount >=
            maxEnemies
        )
        {
            return;
        }


        // นับ Enemy ใกล้ Player
        int nearbyEnemyCount =
            GetNearbyEnemyCount();


        // =================================================
        // ถ้ามี Enemy ใกล้ Player แล้ว
        // ไม่ต้อง Spawn
        // =================================================

        if (
            nearbyEnemyCount >=
            minimumNearbyEnemies
        )
        {
            return;
        }


        // =================================================
        // Cooldown
        // =================================================

        if (
            Time.time <
            nextGuaranteedSpawnTime
        )
        {
            return;
        }


        // =================================================
        // คำนวณจำนวนที่ต้อง Spawn
        // =================================================

        int enemiesNeeded =
            minimumNearbyEnemies -
            nearbyEnemyCount;


        // จำกัดสูงสุดตาม Guaranteed Spawn Amount
        int spawnAmount =
            Mathf.Min(
                enemiesNeeded,
                guaranteedSpawnAmount
            );


        // จำกัดตามจำนวนที่เหลือใน Max Enemies
        int availableEnemySlots =
            maxEnemies -
            currentEnemyCount;


        spawnAmount =
            Mathf.Min(
                spawnAmount,
                availableEnemySlots
            );


        if (
            spawnAmount <= 0
        )
        {
            return;
        }


        // =================================================
        // Spawn Enemy เข้าหา Player
        // =================================================

        int spawnedAmount = 0;


        for (
            int i = 0;
            i < spawnAmount;
            i++
        )
        {
            if (
                SpawnGuaranteedEnemy()
            )
            {
                spawnedAmount++;
            }
        }


        // =================================================
        // ตั้ง Cooldown
        // =================================================

        if (
            spawnedAmount > 0
        )
        {
            nextGuaranteedSpawnTime =
                Time.time +
                guaranteedSpawnCooldown;
        }
    }


    // =====================================================
    // COUNT NEARBY ENEMIES
    // =====================================================

    private int GetNearbyEnemyCount()
    {
        if (player == null)
        {
            return 0;
        }


        Enemy[] enemies =
            FindObjectsByType<Enemy>(
                FindObjectsSortMode.None
            );


        int count = 0;


        for (
            int i = 0;
            i < enemies.Length;
            i++
        )
        {
            Enemy enemy =
                enemies[i];


            if (enemy == null)
            {
                continue;
            }


            float distance =
                Vector2.Distance(
                    player.position,
                    enemy.transform.position
                );


            if (
                distance <=
                densityRadius
            )
            {
                count++;
            }
        }


        return count;
    }


    // =====================================================
    // GUARANTEED SPAWN
    // =====================================================

    private bool SpawnGuaranteedEnemy()
    {
        if (
            enemyPrefabs == null ||
            enemyPrefabs.Length == 0
        )
        {
            Debug.LogWarning(
                "EnemySpawner: ยังไม่ได้ใส่ Enemy Prefab"
            );

            return false;
        }


        if (
            spawnPoints == null ||
            spawnPoints.Length == 0
        )
        {
            Debug.LogWarning(
                "EnemySpawner: ยังไม่ได้ใส่ Enemy Spawn Point"
            );

            return false;
        }


        if (player == null)
        {
            return false;
        }


        // =================================================
        // หาจุด Spawn ที่อยู่ในระยะ
        // =================================================

        List<Transform> validSpawnPoints =
            new List<Transform>();


        for (
            int i = 0;
            i < spawnPoints.Length;
            i++
        )
        {
            Transform point =
                spawnPoints[i];


            if (point == null)
            {
                continue;
            }


            float distance =
                Vector2.Distance(
                    player.position,
                    point.position
                );


            // ใกล้เกินไป
            if (
                distance <
                minimumSpawnDistance
            )
            {
                continue;
            }


            // ไกลเกินไป
            if (
                distance >
                maximumSpawnDistance
            )
            {
                continue;
            }


            validSpawnPoints.Add(
                point
            );
        }


        // ไม่มีจุดที่เหมาะสม
        if (
            validSpawnPoints.Count == 0
        )
        {
            return false;
        }


        // =================================================
        // สุ่ม Enemy
        // =================================================

        int randomEnemyIndex =
            Random.Range(
                0,
                enemyPrefabs.Length
            );


        GameObject selectedEnemy =
            enemyPrefabs[
                randomEnemyIndex
            ];


        // =================================================
        // สุ่ม Spawn Point
        // =================================================

        int randomSpawnIndex =
            Random.Range(
                0,
                validSpawnPoints.Count
            );


        Transform selectedSpawnPoint =
            validSpawnPoints[
                randomSpawnIndex
            ];


        // =================================================
        // Spawn Enemy
        // =================================================

        GameObject newEnemy =
            Instantiate(
                selectedEnemy,
                selectedSpawnPoint.position,
                Quaternion.identity
            );


        // =================================================
        // บังคับให้ Enemy เห็น Player ทันที
        // =================================================

        Enemy enemy =
            newEnemy.GetComponent<Enemy>();


        if (enemy != null)
        {
            enemy.ForceDetectPlayer();
        }


        Debug.Log(
            "Guaranteed Enemy Spawned -> Player"
        );


        return true;
    }


    // =====================================================
    // NORMAL SPAWN ENEMY
    // =====================================================

    private void SpawnEnemy()
    {
        if (
            enemyPrefabs == null ||
            enemyPrefabs.Length == 0
        )
        {
            Debug.LogWarning(
                "EnemySpawner: ยังไม่ได้ใส่ Enemy Prefab"
            );

            return;
        }


        if (
            spawnPoints == null ||
            spawnPoints.Length == 0
        )
        {
            Debug.LogWarning(
                "EnemySpawner: ยังไม่ได้ใส่ Enemy Spawn Point"
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
            enemyPrefabs[
                randomEnemyIndex
            ];


        // สุ่ม Spawn Point
        int randomSpawnIndex =
            Random.Range(
                0,
                spawnPoints.Length
            );


        Transform selectedSpawnPoint =
            spawnPoints[
                randomSpawnIndex
            ];


        if (
            selectedSpawnPoint == null
        )
        {
            Debug.LogWarning(
                "Enemy Spawn Point index " +
                randomSpawnIndex +
                " ยังไม่ได้ใส่ Transform"
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


    // =====================================================
    // INITIAL LIGHT ITEMS
    // =====================================================

    private void SpawnInitialLightItems()
    {
        if (
            lightItemPrefab == null
        )
        {
            Debug.LogWarning(
                "EnemySpawner: ยังไม่ได้ใส่ Light Item Prefab"
            );

            return;
        }


        if (
            itemSpawnPoints == null ||
            itemSpawnPoints.Length == 0
        )
        {
            Debug.LogWarning(
                "EnemySpawner: ยังไม่ได้ใส่ Item Spawn Point"
            );

            return;
        }


        int amountToSpawn =
            Mathf.Min(
                maxLightItems,
                itemSpawnPoints.Length
            );


        for (
            int i = 0;
            i < amountToSpawn;
            i++
        )
        {
            SpawnLightItem();
        }
    }


    // =====================================================
    // UPDATE LIGHT ITEM
    // =====================================================

    private void UpdateLightItemSpawner()
    {
        CleanupDestroyedLightItems();


        while (
            spawnedLightItems.Count <
            maxLightItems
        )
        {
            if (
                !SpawnLightItem()
            )
            {
                break;
            }
        }
    }


    // =====================================================
    // SPAWN LIGHT ITEM
    // =====================================================

    private bool SpawnLightItem()
    {
        if (
            lightItemPrefab == null
        )
        {
            return false;
        }


        if (
            itemSpawnPoints == null ||
            itemSpawnPoints.Length == 0
        )
        {
            return false;
        }


        List<Transform> availablePoints =
            new List<Transform>();


        for (
            int i = 0;
            i < itemSpawnPoints.Length;
            i++
        )
        {
            Transform point =
                itemSpawnPoints[i];


            if (point == null)
            {
                continue;
            }


            if (
                IsLightItemPointOccupied(
                    point
                )
            )
            {
                continue;
            }


            availablePoints.Add(
                point
            );
        }


        if (
            availablePoints.Count == 0
        )
        {
            return false;
        }


        int randomIndex =
            Random.Range(
                0,
                availablePoints.Count
            );


        Transform selectedPoint =
            availablePoints[
                randomIndex
            ];


        GameObject newItem =
            Instantiate(
                lightItemPrefab,
                selectedPoint.position,
                Quaternion.identity
            );


        spawnedLightItems.Add(
            newItem
        );


        return true;
    }


    // =====================================================
    // CHECK LIGHT ITEM POINT
    // =====================================================

    private bool IsLightItemPointOccupied(
        Transform point
    )
    {
        for (
            int i = 0;
            i < spawnedLightItems.Count;
            i++
        )
        {
            GameObject item =
                spawnedLightItems[i];


            if (item == null)
            {
                continue;
            }


            float distance =
                Vector2.Distance(
                    item.transform.position,
                    point.position
                );


            if (
                distance < 0.1f
            )
            {
                return true;
            }
        }


        return false;
    }


    // =====================================================
    // CLEANUP LIGHT ITEMS
    // =====================================================

    private void CleanupDestroyedLightItems()
    {
        for (
            int i =
                spawnedLightItems.Count - 1;
            i >= 0;
            i--
        )
        {
            if (
                spawnedLightItems[i] == null
            )
            {
                spawnedLightItems.RemoveAt(
                    i
                );
            }
        }
    }


    // =====================================================
    // HEALTH POTION DROP
    // =====================================================

    public void TryDropHealthPotion(
        Vector3 dropPosition
    )
    {
        if (
            healthPotionPrefab == null
        )
        {
            Debug.LogWarning(
                "EnemySpawner: ยังไม่ได้ใส่ Health Potion Prefab"
            );

            return;
        }


        float randomValue =
            Random.Range(
                0f,
                100f
            );


        if (
            randomValue >
            healthPotionDropChance
        )
        {
            return;
        }


        Instantiate(
            healthPotionPrefab,
            dropPosition,
            Quaternion.identity
        );


        Debug.Log(
            "Enemy dropped Health Potion!"
        );
    }


    // =====================================================
    // GIZMOS
    // =====================================================

    private void OnDrawGizmosSelected()
    {
        if (player == null)
        {
            return;
        }


        // Density Radius
        Gizmos.DrawWireSphere(
            player.position,
            densityRadius
        );


        // Minimum Spawn Distance
        Gizmos.DrawWireSphere(
            player.position,
            minimumSpawnDistance
        );


        // Maximum Spawn Distance
        Gizmos.DrawWireSphere(
            player.position,
            maximumSpawnDistance
        );
    }
}