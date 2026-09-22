using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum SpriteFacing
    {
        Right,
        Left
    }

    private enum MoveState
    {
        Chasing,
        WallFollowing
    }

    [Header("Health")]
    [SerializeField] private float maxHealth = 50f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("Obstacle Avoidance")]
    [SerializeField] private string wallTag = "Wall";
    [SerializeField] private float obstacleCheckDistance = 1f;
    [Tooltip("ระยะไกลสุดที่เช็คว่ามองเห็นผู้เล่นตรงๆ ได้ไหม (ไม่มีกำแพงบัง) เพื่อตัดสินใจเลิกเกาะกำแพงแล้วพุ่งหาผู้เล่นต่อ")]
    [SerializeField] private float losCheckDistance = 15f;

    [Header("Zigzag Movement")]
    [SerializeField] private bool enableZigzag = false;
    [SerializeField] private float zigzagFrequency = 3f;
    [SerializeField] private float zigzagAmplitude = 1f;

    [Header("Sprite")]
    [SerializeField] private SpriteFacing spriteFacing = SpriteFacing.Right;

    [Header("Attack")]
    [SerializeField] private float contactDamage = 10f;
    [SerializeField] private float attackCooldown = 1f;

    [Header("Hit Flash")]
    [SerializeField] private float flashDuration = 0.08f;

    private float currentHealth;
    private float nextAttackTime;

    private Transform player;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    private Coroutine hitFlashCoroutine;

    // ใช้ offset สุ่มต่อตัว กัน enemy หลายตัวซิกแซกพร้อมกันเป๊ะๆ
    private float zigzagOffset;

    // สถานะการเดิน + ทิศที่เดินล่าสุด (ใช้สืบต่อการเกาะกำแพง)
    private MoveState moveState = MoveState.Chasing;
    private float wallFollowSign = 1f;
    private Vector2 lastMoveDirection = Vector2.right;


    private void Start()
    {
        currentHealth = maxHealth;

        spriteRenderer =
            GetComponentInChildren<SpriteRenderer>();

        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogWarning(
                "Enemy ต้องมี Rigidbody2D (Body Type = Dynamic) ไม่งั้นจะทะลุ Wall และไม่ขยับ"
            );
        }

        zigzagOffset = Random.Range(0f, 100f);

        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning(
                "หา GameObject ที่มี Tag 'Player' ไม่เจอ"
            );
        }
    }


    private void Update()
    {
        FacePlayer();
    }


    private void FixedUpdate()
    {
        MoveTowardsPlayer();
    }


    // =====================================================
    // MOVEMENT
    // =====================================================

    private void MoveTowardsPlayer()
    {
        if (player == null || rb == null)
            return;

        Vector2 toPlayer =
            player.position - transform.position;

        float distanceToPlayer = toPlayer.magnitude;

        if (distanceToPlayer <= 0.05f)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 desiredDirection = toPlayer / distanceToPlayer;

        // ตัดสินใจว่าจะพุ่งตรงหาผู้เล่น หรือเกาะกำแพงไถลไปก่อน
        Vector2 finalDirection =
            GetMoveDirection(desiredDirection, distanceToPlayer);

        // ซิกแซกใช้เฉพาะตอนพุ่งตรงหาผู้เล่นเท่านั้น กันสั่นตอนเกาะกำแพง
        if (enableZigzag && moveState == MoveState.Chasing)
        {
            finalDirection =
                ApplyZigzag(finalDirection);
        }

        lastMoveDirection = finalDirection;

        rb.linearVelocity =
            finalDirection * moveSpeed;
    }


    // =====================================================
    // STATE: CHASING vs WALL FOLLOWING
    // =====================================================

    private Vector2 GetMoveDirection(
        Vector2 desiredDirection,
        float distanceToPlayer
    )
    {
        float losDistance =
            Mathf.Min(distanceToPlayer, losCheckDistance);

        RaycastHit2D losHit =
            Physics2D.Raycast(
                transform.position,
                desiredDirection,
                losDistance
            );

        bool pathBlocked =
            losHit.collider != null &&
            losHit.collider.CompareTag(wallTag);

        // มองเห็นผู้เล่นตรงๆ ไม่มีกำแพงบัง พุ่งตรงได้เลย
        if (!pathBlocked)
        {
            moveState = MoveState.Chasing;
            return AvoidImmediateWall(desiredDirection);
        }

        // เพิ่งเจอกำแพงบังครั้งแรก เลือกฝั่งที่จะเกาะไถล แล้วยึดฝั่งนั้นไว้ตลอด
        if (moveState == MoveState.Chasing)
        {
            Vector2 normal = losHit.normal;

            Vector2 tangentA =
                new Vector2(-normal.y, normal.x);

            float dotA =
                Vector2.Dot(tangentA, desiredDirection);

            wallFollowSign = dotA >= 0f ? 1f : -1f;

            moveState = MoveState.WallFollowing;
        }

        return FollowWall(desiredDirection);
    }


    // เดินตรงแบบปกติ แต่ยังกันชนกำแพงระยะประชิดไว้เผื่อกรณีเพิ่งเลี้ยวกลับมา
    private Vector2 AvoidImmediateWall(Vector2 desiredDirection)
    {
        RaycastHit2D hit =
            Physics2D.Raycast(
                transform.position,
                desiredDirection,
                obstacleCheckDistance
            );

        if (hit.collider == null ||
            !hit.collider.CompareTag(wallTag))
        {
            return desiredDirection;
        }

        Vector2 slideDirection =
            desiredDirection -
            Vector2.Dot(desiredDirection, hit.normal) * hit.normal;

        if (slideDirection.sqrMagnitude < 0.0001f)
        {
            slideDirection =
                new Vector2(-hit.normal.y, hit.normal.x);
        }

        return slideDirection.normalized;
    }


    // เกาะผนังไถลไปทางเดียวกันตลอด (ตาม wallFollowSign) จนกว่าจะเจอทางโล่ง
    private Vector2 FollowWall(Vector2 desiredDirection)
    {
        RaycastHit2D hit =
            Physics2D.Raycast(
                transform.position,
                desiredDirection,
                obstacleCheckDistance
            );

        if (hit.collider == null ||
            !hit.collider.CompareTag(wallTag))
        {
            // ไม่เจอกำแพงตรงหน้าแล้ว ลองเช็คจากทิศที่เดินล่าสุดแทน เพื่อยังคงเกาะผนังต่อ
            hit =
                Physics2D.Raycast(
                    transform.position,
                    lastMoveDirection,
                    obstacleCheckDistance
                );
        }

        if (hit.collider == null ||
            !hit.collider.CompareTag(wallTag))
        {
            // ไม่มีกำแพงใกล้ๆ เลย ปลอดภัยพอจะกลับไปพุ่งตรงหาผู้เล่น
            moveState = MoveState.Chasing;
            return desiredDirection;
        }

        Vector2 tangent =
            wallFollowSign *
            new Vector2(-hit.normal.y, hit.normal.x);

        // เช็คว่าทิศไถลนี้ยังโดนกำแพงบังอยู่ไหม (เช่นเจอมุมเหลี่ยม) ถ้าใช่ให้โค้งตามผิวกำแพงเพิ่ม
        RaycastHit2D tangentHit =
            Physics2D.Raycast(
                transform.position,
                tangent,
                obstacleCheckDistance * 0.5f
            );

        if (tangentHit.collider != null &&
            tangentHit.collider.CompareTag(wallTag))
        {
            tangent =
                (tangent + hit.normal * 0.5f).normalized;
        }

        return tangent;
    }


    // =====================================================
    // ZIGZAG MOVEMENT
    // =====================================================

    private Vector2 ApplyZigzag(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.0001f)
            return direction;

        // แกนตั้งฉากกับทิศที่เดิน ใช้เป็นแกนส่าย
        Vector2 perpendicular =
            new Vector2(-direction.y, direction.x);

        float sway =
            Mathf.Sin(
                (Time.time + zigzagOffset) * zigzagFrequency
            ) * zigzagAmplitude;

        Vector2 zigzagDirection =
            direction + perpendicular * sway;

        return zigzagDirection.normalized;
    }


    // =====================================================
    // FACE PLAYER
    // =====================================================

    private void FacePlayer()
    {
        if (player == null)
            return;

        if (spriteRenderer == null)
            return;

        float directionX =
            player.position.x -
            transform.position.x;

        if (Mathf.Abs(directionX) <= 0.01f)
            return;


        // Sprite ต้นฉบับหันขวา
        if (spriteFacing == SpriteFacing.Right)
        {
            if (directionX > 0f)
            {
                spriteRenderer.flipX = false;
            }
            else
            {
                spriteRenderer.flipX = true;
            }
        }

        // Sprite ต้นฉบับหันซ้าย
        else
        {
            if (directionX < 0f)
            {
                spriteRenderer.flipX = false;
            }
            else
            {
                spriteRenderer.flipX = true;
            }
        }
    }


    // =====================================================
    // HEALTH
    // =====================================================

    public void TakeDamage(float damage)
    {
        if (damage <= 0f)
            return;

        currentHealth -= damage;

        currentHealth =
            Mathf.Clamp(
                currentHealth,
                0f,
                maxHealth
            );

        Debug.Log(
            "Enemy HP: " +
            currentHealth +
            " / " +
            maxHealth
        );

        if (hitFlashCoroutine != null)
        {
            StopCoroutine(hitFlashCoroutine);
        }

        hitFlashCoroutine =
            StartCoroutine(HitFlash());

        if (currentHealth <= 0f)
        {
            Die();
        }
    }


    // =====================================================
    // HIT FLASH
    // =====================================================

    private IEnumerator HitFlash()
    {
        if (spriteRenderer == null)
            yield break;

        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(
            flashDuration
        );

        spriteRenderer.color = Color.white;

        hitFlashCoroutine = null;
    }


    // =====================================================
    // ATTACK PLAYER
    // =====================================================

    private void OnCollisionEnter2D(
        Collision2D collision
    )
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        TryAttack(collision.gameObject);
    }


    private void OnCollisionStay2D(
        Collision2D collision
    )
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        TryAttack(collision.gameObject);
    }


    private void TryAttack(GameObject target)
    {
        if (Time.time < nextAttackTime)
            return;

        PlayerController playerController =
            target.GetComponent<PlayerController>();

        if (playerController == null)
            return;

        Vector2 knockbackDirection =
            target.transform.position -
            transform.position;

        playerController.TakeDamage(
            contactDamage,
            knockbackDirection
        );

        nextAttackTime =
            Time.time + attackCooldown;
    }


    // =====================================================
    // DEATH
    // =====================================================

    private void Die()
    {
        Destroy(gameObject);
    }
}