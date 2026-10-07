using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // =====================================================
    // SPRITE FACING
    // =====================================================

    public enum SpriteFacing
    {
        Right,
        Left,
        Up,
        Down
    }


    // =====================================================
    // MOVE STATE
    // =====================================================

    private enum MoveState
    {
        Chasing,
        WallFollowing
    }


    // =====================================================
    // SCORE
    // =====================================================

    [Header("Score")]
    [SerializeField] private int scoreValue = 100;


    // =====================================================
    // HEALTH
    // =====================================================

    [Header("Health")]
    [SerializeField] private float maxHealth = 50f;


    // =====================================================
    // MOVEMENT
    // =====================================================

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;


    // =====================================================
    // PLAYER DETECTION
    // =====================================================

    [Header("Player Detection")]
    [SerializeField] private float detectionRange = 8f;


    // =====================================================
    // COMBAT DISTANCE
    // =====================================================

    [Header("Combat Distance")]
    [SerializeField] private float stopDistance = 1.5f;

    [SerializeField] private float attackRange = 2f;


    // =====================================================
    // OBSTACLE AVOIDANCE
    // =====================================================

    [Header("Obstacle Avoidance")]
    [SerializeField] private string wallTag = "Wall";

    [SerializeField] private float obstacleCheckDistance = 1f;

    [SerializeField] private float losCheckDistance = 15f;


    // =====================================================
    // ZIGZAG
    // =====================================================

    [Header("Zigzag Movement")]
    [SerializeField] private bool enableZigzag = false;

    [SerializeField] private float zigzagFrequency = 3f;

    [SerializeField] private float zigzagAmplitude = 1f;


    // =====================================================
    // SPRITE
    // =====================================================

    [Header("Sprite")]
    [SerializeField]
    private SpriteFacing spriteFacing =
        SpriteFacing.Right;


    // =====================================================
    // ATTACK
    // =====================================================

    [Header("Attack")]
    [SerializeField] private float contactDamage = 10f;

    [SerializeField] private float attackCooldown = 1f;

    private bool isDead = false;


    // =====================================================
    // SOUND
    // =====================================================

    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip detectSound;

    [SerializeField] private AudioClip attackSound;

    [SerializeField] private AudioClip hitSound;

    [SerializeField] private AudioClip deathSound;


    [Header("Movement Sound")]
    [SerializeField] private AudioClip moveSound;

    [SerializeField] private float moveSoundInterval = 0.5f;

    private float nextMoveSoundTime;


    // =====================================================
    // ATTACK ANIMATION
    // =====================================================

    [Header("Attack Animation")]
    [SerializeField] private bool useAttackAnimation = true;

    [SerializeField] private string attackTriggerName = "Attack";

    [SerializeField] private string attackStateName = "Knight_Attack";

    [SerializeField] private string walkStateName = "Knight_Walk";


    // =====================================================
    // HIT FLASH
    // =====================================================

    [Header("Hit Flash")]
    [SerializeField] private float flashDuration = 0.08f;


    // =====================================================
    // EXP DROP
    // =====================================================

    [Header("EXP Drop")]
    [SerializeField] private GameObject expOrbPrefab;

    [SerializeField] private int expOrbAmount = 3;

    [SerializeField] private float expPerOrb = 10f;

    [SerializeField] private float expDropRadius = 0.5f;


    // =====================================================
    // PRIVATE VARIABLES
    // =====================================================

    private float currentHealth;

    private float nextAttackTime;

    private Transform player;

    private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;

    private Animator animator;

    private Coroutine hitFlashCoroutine;

    private float zigzagOffset;

    private MoveState moveState =
        MoveState.Chasing;

    private float wallFollowSign = 1f;

    private Vector2 lastMoveDirection =
        Vector2.right;

    private EnemyLightZone currentLightZone;


    // =====================================================
    // DETECTION STATE
    // =====================================================

    private bool hasDetectedPlayer = false;


    // =====================================================
    // ATTACK STATE
    // =====================================================

    private bool isAttacking = false;

    private bool hasAttackHit = false;


    // =====================================================
    // SOUND
    // =====================================================

    private void PlaySound(AudioClip clip)
    {
        if (audioSource == null)
            return;

        if (clip == null)
            return;

        audioSource.PlayOneShot(clip);
    }


    // =====================================================
    // START
    // =====================================================

    private void Start()
    {
        currentHealth =
            maxHealth;


        spriteRenderer =
            GetComponentInChildren<SpriteRenderer>();


        rb =
            GetComponent<Rigidbody2D>();


        animator =
            GetComponentInChildren<Animator>();


        if (rb == null)
        {
            Debug.LogWarning(
                gameObject.name +
                " ไม่มี Rigidbody2D"
            );
        }


        if (animator == null &&
            useAttackAnimation)
        {
            Debug.LogWarning(
                gameObject.name +
                " ไม่มี Animator"
            );
        }


        zigzagOffset =
            Random.Range(
                0f,
                100f
            );


        GameObject playerObject =
            GameObject.FindGameObjectWithTag(
                "Player"
            );


        if (playerObject != null)
        {
            player =
                playerObject.transform;
        }
        else
        {
            Debug.LogWarning(
                "หา GameObject ที่มี Tag Player ไม่เจอ"
            );
        }
    }


    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        if (isDead)
            return;


        DetectPlayer();

        FacePlayer();

        CheckAttackRange();
    }


    // =====================================================
    // FIXED UPDATE
    // =====================================================

    private void FixedUpdate()
    {
        if (isDead)
            return;


        MoveTowardsPlayer();


        if (
            hasDetectedPlayer &&
            !isAttacking &&
            Time.time >= nextMoveSoundTime &&
            rb != null &&
            rb.linearVelocity.magnitude > 0.1f
        )
        {
            PlaySound(moveSound);


            nextMoveSoundTime =
                Time.time + moveSoundInterval;
        }
    }


    // =====================================================
    // DETECT PLAYER
    // =====================================================

    private void DetectPlayer()
    {
        if (player == null)
            return;


        if (hasDetectedPlayer)
            return;


        float distanceToPlayer =
            Vector2.Distance(
                transform.position,
                player.position
            );


        if (
            distanceToPlayer <=
            detectionRange
        )
        {
            hasDetectedPlayer =
                true;


            PlaySound(detectSound);


            Debug.Log(
                gameObject.name +
                " detected Player!"
            );
        }
    }


    // =====================================================
    // FORCE DETECT PLAYER
    // =====================================================

    public void ForceDetectPlayer()
    {
        if (isDead)
            return;


        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag(
                    "Player"
                );


            if (playerObject != null)
            {
                player =
                    playerObject.transform;
            }
        }


        if (player == null)
            return;


        if (hasDetectedPlayer)
            return;


        hasDetectedPlayer =
            true;


        PlaySound(detectSound);


        Debug.Log(
            gameObject.name +
            " FORCE DETECTED PLAYER!"
        );
    }


    // =====================================================
    // MOVE
    // =====================================================

    private void MoveTowardsPlayer()
    {
        if (
            player == null ||
            rb == null
        )
        {
            return;
        }


        if (!hasDetectedPlayer)
        {
            rb.linearVelocity =
                Vector2.zero;

            return;
        }


        if (isAttacking)
        {
            rb.linearVelocity =
                Vector2.zero;

            return;
        }


        Vector2 toPlayer =
            player.position -
            transform.position;


        float distanceToPlayer =
            toPlayer.magnitude;


        if (
            distanceToPlayer <=
            stopDistance
        )
        {
            rb.linearVelocity =
                Vector2.zero;

            return;
        }


        Vector2 desiredDirection =
            toPlayer.normalized;


        Vector2 finalDirection =
            GetMoveDirection(
                desiredDirection,
                distanceToPlayer
            );


        if (
            enableZigzag &&
            moveState ==
            MoveState.Chasing
        )
        {
            finalDirection =
                ApplyZigzag(
                    finalDirection
                );
        }


        EnemyLightZone lightZone =
            EnemyLightZone.Instance;


        if (lightZone != null)
        {
            Vector2 nextPosition =
                rb.position +
                finalDirection *
                moveSpeed *
                Time.fixedDeltaTime;


            bool blocked =
                lightZone.ShouldBlockPosition(
                    this,
                    rb.position,
                    nextPosition
                );


            if (blocked)
            {
                rb.linearVelocity =
                    Vector2.zero;

                return;
            }
        }


        lastMoveDirection =
            finalDirection;


        rb.linearVelocity =
            finalDirection *
            moveSpeed;
    }


    // =====================================================
    // ATTACK RANGE
    // =====================================================

    private void CheckAttackRange()
    {
        if (player == null)
            return;


        if (!hasDetectedPlayer)
            return;


        if (isAttacking)
            return;


        if (
            Time.time <
            nextAttackTime
        )
        {
            return;
        }


        float distanceToPlayer =
            Vector2.Distance(
                transform.position,
                player.position
            );


        if (
            distanceToPlayer <=
            attackRange
        )
        {
            TryAttack(
                player.gameObject
            );
        }
    }


    // =====================================================
    // GET MOVE DIRECTION
    // =====================================================

    private Vector2 GetMoveDirection(
        Vector2 desiredDirection,
        float distanceToPlayer
    )
    {
        float losDistance =
            Mathf.Min(
                distanceToPlayer,
                losCheckDistance
            );


        RaycastHit2D losHit =
            Physics2D.Raycast(
                transform.position,
                desiredDirection,
                losDistance
            );


        bool pathBlocked =
            losHit.collider != null &&
            losHit.collider.CompareTag(
                wallTag
            );


        if (!pathBlocked)
        {
            moveState =
                MoveState.Chasing;


            return AvoidImmediateWall(
                desiredDirection
            );
        }


        if (
            moveState ==
            MoveState.Chasing
        )
        {
            Vector2 normal =
                losHit.normal;


            Vector2 tangentA =
                new Vector2(
                    -normal.y,
                    normal.x
                );


            float dotA =
                Vector2.Dot(
                    tangentA,
                    desiredDirection
                );


            wallFollowSign =
                dotA >= 0f
                    ? 1f
                    : -1f;


            moveState =
                MoveState.WallFollowing;
        }


        return FollowWall(
            desiredDirection
        );
    }


    // =====================================================
    // AVOID WALL
    // =====================================================

    private Vector2 AvoidImmediateWall(
        Vector2 desiredDirection
    )
    {
        RaycastHit2D hit =
            Physics2D.Raycast(
                transform.position,
                desiredDirection,
                obstacleCheckDistance
            );


        if (
            hit.collider == null ||
            !hit.collider.CompareTag(
                wallTag
            )
        )
        {
            return desiredDirection;
        }


        Vector2 slideDirection =
            desiredDirection -
            Vector2.Dot(
                desiredDirection,
                hit.normal
            ) *
            hit.normal;


        if (
            slideDirection.sqrMagnitude <
            0.0001f
        )
        {
            slideDirection =
                new Vector2(
                    -hit.normal.y,
                    hit.normal.x
                );
        }


        return slideDirection.normalized;
    }


    // =====================================================
    // FOLLOW WALL
    // =====================================================

    private Vector2 FollowWall(
        Vector2 desiredDirection
    )
    {
        RaycastHit2D hit =
            Physics2D.Raycast(
                transform.position,
                desiredDirection,
                obstacleCheckDistance
            );


        if (
            hit.collider == null ||
            !hit.collider.CompareTag(
                wallTag
            )
        )
        {
            hit =
                Physics2D.Raycast(
                    transform.position,
                    lastMoveDirection,
                    obstacleCheckDistance
                );
        }


        if (
            hit.collider == null ||
            !hit.collider.CompareTag(
                wallTag
            )
        )
        {
            moveState =
                MoveState.Chasing;


            return desiredDirection;
        }


        Vector2 tangent =
            wallFollowSign *
            new Vector2(
                -hit.normal.y,
                hit.normal.x
            );


        RaycastHit2D tangentHit =
            Physics2D.Raycast(
                transform.position,
                tangent,
                obstacleCheckDistance *
                0.5f
            );


        if (
            tangentHit.collider != null &&
            tangentHit.collider.CompareTag(
                wallTag
            )
        )
        {
            tangent =
                (
                    tangent +
                    hit.normal * 0.5f
                ).normalized;
        }


        return tangent;
    }


    // =====================================================
    // ZIGZAG
    // =====================================================

    private Vector2 ApplyZigzag(
        Vector2 direction
    )
    {
        if (
            direction.sqrMagnitude <
            0.0001f
        )
        {
            return direction;
        }


        Vector2 perpendicular =
            new Vector2(
                -direction.y,
                direction.x
            );


        float sway =
            Mathf.Sin(
                (
                    Time.time +
                    zigzagOffset
                ) *
                zigzagFrequency
            ) *
            zigzagAmplitude;


        Vector2 zigzagDirection =
            direction +
            perpendicular *
            sway;


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


        if (!hasDetectedPlayer)
            return;


        float directionX =
            player.position.x -
            transform.position.x;


        if (
            Mathf.Abs(directionX) <=
            0.01f
        )
        {
            return;
        }


        if (
            spriteFacing ==
            SpriteFacing.Right
        )
        {
            spriteRenderer.flipX =
                directionX < 0f;
        }
        else if (
            spriteFacing ==
            SpriteFacing.Left
        )
        {
            spriteRenderer.flipX =
                directionX > 0f;
        }
        else
        {
            spriteRenderer.flipX =
                false;
        }
    }


    // =====================================================
    // TRY ATTACK
    // =====================================================

    private void TryAttack(
        GameObject target
    )
    {
        if (target == null)
            return;


        if (isAttacking)
            return;


        if (
            Time.time <
            nextAttackTime
        )
        {
            return;
        }


        PlayerController playerController =
            target.GetComponent<PlayerController>();


        if (playerController == null)
            return;


        float distanceToPlayer =
            Vector2.Distance(
                transform.position,
                target.transform.position
            );


        if (
            distanceToPlayer >
            attackRange
        )
        {
            return;
        }


        if (
            useAttackAnimation &&
            animator != null
        )
        {
            StartAttack();
        }
        else
        {
            DealDamage(
                playerController
            );


            nextAttackTime =
                Time.time +
                attackCooldown;
        }
    }


    // =====================================================
    // START ATTACK
    // =====================================================

    private void StartAttack()
    {
        isAttacking =
            true;


        hasAttackHit =
            false;


        if (rb != null)
        {
            rb.linearVelocity =
                Vector2.zero;
        }


        PlaySound(attackSound);


        animator.ResetTrigger(
            attackTriggerName
        );


        animator.SetTrigger(
            attackTriggerName
        );
    }


    // =====================================================
    // ANIMATION EVENT
    // =====================================================

    public void AttackHit()
    {
        if (!isAttacking)
            return;


        if (hasAttackHit)
            return;


        if (player == null)
            return;


        float distanceToPlayer =
            Vector2.Distance(
                transform.position,
                player.position
            );


        if (
            distanceToPlayer >
            attackRange
        )
        {
            return;
        }


        PlayerController playerController =
            player.GetComponent<PlayerController>();


        if (playerController == null)
            return;


        DealDamage(
            playerController
        );


        hasAttackHit =
            true;
    }


    // =====================================================
    // ATTACK FINISHED
    // =====================================================

    public void AttackFinished()
    {
        if (!isAttacking)
            return;


        FinishAttack();
    }


    // =====================================================
    // DEAL DAMAGE
    // =====================================================

    private void DealDamage(
        PlayerController playerController
    )
    {
        if (playerController == null)
            return;


        if (player == null)
            return;


        Vector2 knockbackDirection =
            player.position -
            transform.position;


        playerController.TakeDamage(
            contactDamage,
            knockbackDirection
        );


        Debug.Log(
            gameObject.name +
            " attacked Player for " +
            contactDamage +
            " damage"
        );
    }


    // =====================================================
    // FINISH ATTACK
    // =====================================================

    private void FinishAttack()
    {
        isAttacking =
            false;


        hasAttackHit =
            false;


        nextAttackTime =
            Time.time +
            attackCooldown;


        PlayWalkAnimation();
    }


    // =====================================================
    // PLAY WALK
    // =====================================================

    private void PlayWalkAnimation()
    {
        if (animator == null)
            return;


        animator.ResetTrigger(
            attackTriggerName
        );


        animator.CrossFade(
            walkStateName,
            0.05f,
            0
        );
    }


    // =====================================================
    // TAKE DAMAGE
    // =====================================================

    public void TakeDamage(
        float damage
    )
    {
        if (damage <= 0f)
            return;


        // ถ้าตายแล้ว ไม่รับ Damage อีก
        if (isDead)
            return;


        currentHealth -=
            damage;


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


        // Hit Flash
        if (hitFlashCoroutine != null)
        {
            StopCoroutine(
                hitFlashCoroutine
            );
        }


        hitFlashCoroutine =
            StartCoroutine(
                HitFlash()
            );


        // =================================================
        // DEATH
        // =================================================

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


        spriteRenderer.color =
            Color.red;


        yield return new WaitForSeconds(
            flashDuration
        );


        // ถ้าตายแล้ว ไม่ต้องเปลี่ยนกลับ
        if (isDead)
            yield break;


        spriteRenderer.color =
            Color.white;


        hitFlashCoroutine =
            null;
    }


    // =====================================================
    // LIGHT ZONE
    // =====================================================

    public void EnterLightZone(
        EnemyLightZone lightZone
    )
    {
        if (lightZone == null)
            return;


        currentLightZone =
            lightZone;
    }


    public void ExitLightZone(
        EnemyLightZone lightZone
    )
    {
        if (
            currentLightZone ==
            lightZone
        )
        {
            currentLightZone =
                null;
        }
    }


    // =====================================================
    // COLLISION
    // =====================================================

    private void OnCollisionEnter2D(
        Collision2D collision
    )
    {
        if (
            !collision.gameObject.CompareTag(
                "Player"
            )
        )
        {
            return;
        }

        // Damage ใช้ AttackHit()
    }


    private void OnCollisionStay2D(
        Collision2D collision
    )
    {
        if (
            !collision.gameObject.CompareTag(
                "Player"
            )
        )
        {
            return;
        }

        // Damage ใช้ AttackHit()
    }


    // =====================================================
    // DROP EXP
    // =====================================================

    private void DropExp()
    {
        if (expOrbPrefab == null)
        {
            Debug.LogWarning(
                gameObject.name +
                " ไม่มี Exp Orb Prefab"
            );

            return;
        }


        if (expOrbAmount <= 0)
            return;


        for (
            int i = 0;
            i < expOrbAmount;
            i++
        )
        {
            // สุ่มทิศทางรอบตัว
            Vector2 randomDirection =
                Random.insideUnitCircle.normalized;


            // สุ่มระยะกระจาย
            float randomDistance =
                Random.Range(
                    0.5f,
                    expDropRadius
                );


            Vector2 offset =
                randomDirection *
                randomDistance;


            Vector3 spawnPosition =
                transform.position +
                new Vector3(
                    offset.x,
                    offset.y,
                    0f
                );


            GameObject orb =
                Instantiate(
                    expOrbPrefab,
                    spawnPosition,
                    Quaternion.identity
                );


            ExpOrb expOrb =
                orb.GetComponent<ExpOrb>();


            if (expOrb != null)
            {
                expOrb.SetExpAmount(
                    expPerOrb
                );
            }
        }
    }


    // =====================================================
    // DIE
    // =====================================================

    private void Die()
    {
        // ป้องกัน Die() ทำงานซ้ำ
        if (isDead)
            return;


        isDead = true;


        // =================================================
        // ADD SCORE
        // =================================================

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(
                scoreValue
            );

            ScoreManager.Instance.AddEnemyDefeated();
        }


        // =================================================
        // REMOVE FROM LIGHT ZONE
        // =================================================

        if (currentLightZone != null)
        {
            currentLightZone.RemoveEnemy(
                this
            );

            currentLightZone = null;
        }


        // =================================================
        // DROP EXP
        // =================================================

        DropExp();


        // =================================================
        // DROP HEALTH POTION
        // =================================================

        EnemySpawner enemySpawner =
            FindFirstObjectByType<EnemySpawner>();


        if (enemySpawner != null)
        {
            enemySpawner.TryDropHealthPotion(
                transform.position
            );
        }


        // =================================================
        // DESTROY IMMEDIATELY
        // =================================================

        Destroy(gameObject);
    }


    // =====================================================
    // GIZMOS
    // =====================================================

    private void OnDrawGizmosSelected()
    {
        // Detection
        Gizmos.DrawWireSphere(
            transform.position,
            detectionRange
        );


        // Stop
        Gizmos.DrawWireSphere(
            transform.position,
            stopDistance
        );


        // Attack
        Gizmos.DrawWireSphere(
            transform.position,
            attackRange
        );
    }
}