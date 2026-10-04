using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ExpOrb : MonoBehaviour
{
    [Header("EXP")]
    [SerializeField] private float expAmount = 10f;


    [Header("Magnet")]
    [SerializeField] private float pickupRange = 2.5f;

    [SerializeField] private float moveSpeed = 8f;

    [SerializeField] private float acceleration = 20f;


    [Header("Random Color")]
    [SerializeField] private Color[] randomColors;


    [Header("Glow")]
    [SerializeField] private Light2D orbLight;

    [SerializeField] private float lightIntensity = 1f;

    [SerializeField] private float lightOuterRadius = 1f;

    [SerializeField] private float lightInnerRadius = 0f;


    // =====================================================
    // PRIVATE
    // =====================================================

    private Transform player;

    private PlayerLevel playerLevel;

    private float currentSpeed = 0f;

    private bool isMovingToPlayer = false;


    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        // หา Player
        GameObject playerObject =
            GameObject.FindGameObjectWithTag(
                "Player"
            );


        if (playerObject != null)
        {
            player =
                playerObject.transform;


            playerLevel =
                playerObject.GetComponent<PlayerLevel>();


            if (playerLevel == null)
            {
                playerLevel =
                    playerObject.GetComponentInParent<PlayerLevel>();
            }
        }


        // สุ่มสี
        RandomizeColor();


        // ตั้งค่า Light
        SetupLight();
    }


    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        if (player == null)
            return;


        float distance =
            Vector2.Distance(
                transform.position,
                player.position
            );


        // Player เข้าใกล้
        if (distance <= pickupRange)
        {
            isMovingToPlayer = true;
        }


        // เริ่มดูดเข้าหา Player
        if (isMovingToPlayer)
        {
            MoveToPlayer();
        }
    }


    // =====================================================
    // MOVE TO PLAYER
    // =====================================================

    private void MoveToPlayer()
    {
        if (player == null)
            return;


        // ค่อย ๆ เพิ่มความเร็ว
        currentSpeed =
            Mathf.MoveTowards(
                currentSpeed,
                moveSpeed,
                acceleration *
                Time.deltaTime
            );


        // เคลื่อนที่เข้าหา Player
        transform.position =
            Vector2.MoveTowards(
                transform.position,
                player.position,
                currentSpeed *
                Time.deltaTime
            );


        // เช็กว่าเข้า Player แล้วหรือยัง
        float distance =
            Vector2.Distance(
                transform.position,
                player.position
            );


        if (distance <= 0.1f)
        {
            Collect();
        }
    }


    // =====================================================
    // COLLECT
    // =====================================================

    private void Collect()
    {
        if (playerLevel != null)
        {
            playerLevel.AddExp(
                expAmount
            );
        }
        else
        {
            Debug.LogWarning(
                "ไม่พบ PlayerLevel บน Player"
            );
        }


        Destroy(gameObject);
    }


    // =====================================================
    // SET EXP
    // =====================================================

    public void SetExpAmount(
        float amount
    )
    {
        expAmount = amount;
    }


    // =====================================================
    // RANDOM COLOR
    // =====================================================

    private void RandomizeColor()
    {
        SpriteRenderer spriteRenderer =
            GetComponent<SpriteRenderer>();


        if (spriteRenderer == null)
        {
            Debug.LogWarning(
                "ExpOrb ต้องมี SpriteRenderer"
            );

            return;
        }


        if (
            randomColors == null ||
            randomColors.Length == 0
        )
        {
            Debug.LogWarning(
                "ExpOrb ยังไม่ได้ใส่ Random Colors"
            );

            return;
        }


        // สุ่ม Index
        int randomIndex =
            Random.Range(
                0,
                randomColors.Length
            );


        // สีที่สุ่มได้
        Color orbColor =
            randomColors[randomIndex];


        // เปลี่ยนสี Sprite
        spriteRenderer.color =
            orbColor;


        // เปลี่ยนสี Light ให้เหมือน Orb
        if (orbLight != null)
        {
            orbLight.color =
                orbColor;
        }
    }


    // =====================================================
    // SETUP LIGHT
    // =====================================================

    private void SetupLight()
    {
        if (orbLight == null)
            return;


        orbLight.intensity =
            lightIntensity;


        orbLight.pointLightOuterRadius =
            lightOuterRadius;


        orbLight.pointLightInnerRadius =
            lightInnerRadius;
    }


    // =====================================================
    // GIZMOS
    // =====================================================

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            pickupRange
        );
    }
}