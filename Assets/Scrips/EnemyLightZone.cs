using System.Collections.Generic;
using UnityEngine;

public class EnemyLightZone : MonoBehaviour
{
    public static EnemyLightZone Instance;


    [Header("Light Area Settings")]
    [SerializeField]
    private int maxEnemiesInLight = 3;


    [SerializeField]
    private CircleCollider2D lightCollider;


    // =====================================================
    // AWAKE
    // =====================================================

    private void Awake()
    {
        Instance =
            this;

        if (lightCollider == null)
        {
            lightCollider =
                GetComponent<CircleCollider2D>();
        }

        if (lightCollider == null)
        {
            Debug.LogError(
                "EnemyLightZone ต้องมี CircleCollider2D"
            );
        }
    }


    // =====================================================
    // CHECK IF POSITION IS INSIDE LIGHT
    // =====================================================

    private bool IsInsideLight(
        Vector2 position
    )
    {
        if (lightCollider == null)
            return false;


        Vector2 center =
            transform.TransformPoint(
                lightCollider.offset
            );


        float radius =
            lightCollider.radius *
            Mathf.Max(
                transform.lossyScale.x,
                transform.lossyScale.y
            );


        return Vector2.Distance(
            position,
            center
        ) <= radius;
    }


    // =====================================================
    // GET ALL ENEMIES INSIDE LIGHT
    // =====================================================

    public int GetEnemiesInsideLight()
    {
        if (lightCollider == null)
            return 0;


        Vector2 center =
            transform.TransformPoint(
                lightCollider.offset
            );


        float radius =
            lightCollider.radius *
            Mathf.Max(
                transform.lossyScale.x,
                transform.lossyScale.y
            );


        Collider2D[] colliders =
            Physics2D.OverlapCircleAll(
                center,
                radius
            );


        HashSet<Enemy> enemies =
            new HashSet<Enemy>();


        foreach (
            Collider2D collider
            in colliders
        )
        {
            if (collider == null)
                continue;


            Enemy enemy =
                collider.GetComponentInParent<Enemy>();


            if (enemy == null)
                continue;


            enemies.Add(
                enemy
            );
        }


        return enemies.Count;
    }


    // =====================================================
    // CHECK NEXT POSITION
    // =====================================================

    public bool ShouldBlockPosition(
        Enemy enemy,
        Vector2 currentPosition,
        Vector2 nextPosition
    )
    {
        if (enemy == null)
            return false;


        if (lightCollider == null)
            return false;


        // -----------------------------------------------
        // Enemy อยู่ในแสงแล้ว
        // -----------------------------------------------

        if (IsInsideLight(
            currentPosition
        ))
        {
            return false;
        }


        // -----------------------------------------------
        // ยังไม่ได้กำลังจะเข้าแสง
        // -----------------------------------------------

        if (!IsInsideLight(
            nextPosition
        ))
        {
            return false;
        }


        // -----------------------------------------------
        // กำลังจะเข้าแสง
        // -----------------------------------------------

        int currentEnemyCount =
            GetEnemiesInsideLight();


        // -----------------------------------------------
        // ยังไม่ถึง 3 ตัว
        // -----------------------------------------------

        if (currentEnemyCount <
            maxEnemiesInLight)
        {
            return false;
        }


        // -----------------------------------------------
        // มีครบ 3 ตัวแล้ว
        // ห้ามเข้า
        // -----------------------------------------------

        return true;
    }


    // =====================================================
    // CAN ENTER
    // =====================================================

    public bool CanEnemyEnter(
        Enemy enemy
    )
    {
        if (enemy == null)
            return false;


        if (IsInsideLight(
            enemy.transform.position
        ))
        {
            return true;
        }


        return GetEnemiesInsideLight() <
               maxEnemiesInLight;
    }


    // =====================================================
    // ADD ENEMY
    // =====================================================

    public void AddEnemy(
        Enemy enemy
    )
    {
        if (enemy == null)
            return;


        if (!CanEnemyEnter(enemy))
            return;


        enemy.EnterLightZone(
            this
        );
    }


    // =====================================================
    // REMOVE ENEMY
    // =====================================================

    public void RemoveEnemy(
        Enemy enemy
    )
    {
        if (enemy == null)
            return;


        enemy.ExitLightZone(
            this
        );
    }


    // =====================================================
    // GET COUNT
    // =====================================================

    public int GetEnemyCount()
    {
        return GetEnemiesInsideLight();
    }


    // =====================================================
    // GIZMOS
    // =====================================================

    private void OnDrawGizmosSelected()
    {
        CircleCollider2D circle =
            lightCollider;


        if (circle == null)
        {
            circle =
                GetComponent<CircleCollider2D>();
        }


        if (circle == null)
            return;


        Vector2 center =
            transform.TransformPoint(
                circle.offset
            );


        float radius =
            circle.radius *
            Mathf.Max(
                transform.lossyScale.x,
                transform.lossyScale.y
            );


        Gizmos.DrawWireSphere(
            center,
            radius
        );
    }
}