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

    [SerializeField] private int maxEnemies = 5;


    private float nextSpawnTime;


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
        // Spawn Light Item ตอนเริ่มเกม
        SpawnInitialLightItems();
    }


    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        // Enemy Spawn
        UpdateEnemySpawner();


        // Light Item Spawn
        UpdateLightItemSpawner();
    }


    // =====================================================
    // ENEMY SPAWNER
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
    // SPAWN ENEMY
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
                "Enemy Spawn Point index "
                + randomSpawnIndex
                + " ยังไม่ได้ใส่ Transform"
            );

            return;
        }


        // =================================================
        // สร้าง Enemy
        // =================================================

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


        // ถ้า Item หายไป
        // ให้สร้างใหม่จนกว่าจะครบจำนวนสูงสุด

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


        // =================================================
        // หาจุดที่ยังไม่มี Item
        // =================================================

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


        // =================================================
        // สุ่มจุด
        // =================================================

        int randomIndex =
            Random.Range(
                0,
                availablePoints.Count
            );


        Transform selectedPoint =
            availablePoints[
                randomIndex
            ];


        // =================================================
        // Spawn
        // =================================================

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
        // =================================================
        // ไม่มี Potion Prefab
        // =================================================

        if (
            healthPotionPrefab == null
        )
        {
            Debug.LogWarning(
                "EnemySpawner: ยังไม่ได้ใส่ Health Potion Prefab"
            );

            return;
        }


        // =================================================
        // สุ่มโอกาส
        // =================================================

        float randomValue =
            Random.Range(
                0f,
                100f
            );


        // =================================================
        // ไม่ผ่านโอกาสดรอป
        // =================================================

        if (
            randomValue >
            healthPotionDropChance
        )
        {
            return;
        }


        // =================================================
        // Spawn Potion
        // =================================================

        Instantiate(
            healthPotionPrefab,
            dropPosition,
            Quaternion.identity
        );


        Debug.Log(
            "Enemy dropped Health Potion!"
        );
    }
}