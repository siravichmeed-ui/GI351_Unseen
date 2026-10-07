using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Hit Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hitSound;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;

    [Header("Blackout Damage")]
    [SerializeField] private float blackoutDamage = 5f;
    [SerializeField] private float blackoutDamageInterval = 1f;

    [Header("Hit Effect")]
    [SerializeField] private float hitFlashDuration = 0.08f;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float hitInvincibilityTime = 0.5f;

    [Header("Player Sprite")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    private float currentHealth;
    private float invincibilityTimer;

    private float blackoutDamageTimer;

    private Rigidbody2D rb;
    private Animator animator;

    private Vector2 movement;

    private Coroutine hitFlashCoroutine;

    private bool isUIOpen = false;
    private bool isDead = false;

    private LampManager lampManager;

    // =========================
    // PUBLIC
    // =========================

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    public bool IsDead => isDead;

    // =========================
    // AWAKE
    // =========================

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        currentHealth = maxHealth;

        lampManager =
            FindFirstObjectByType<LampManager>();
    }

    // =========================
    // UPDATE
    // =========================

    private void Update()
    {
        // ถ้าตายแล้ว
        if (isDead)
        {
            movement = Vector2.zero;

            if (animator != null)
            {
                animator.SetBool(
                    "IsMoving",
                    false
                );
            }

            return;
        }

        // =========================================
        // BLACKOUT DAMAGE
        // =========================================

        HandleBlackoutDamage();

        // ถ้าเปิด UI อยู่
        if (isUIOpen)
        {
            movement = Vector2.zero;

            if (animator != null)
            {
                animator.SetBool(
                    "IsMoving",
                    false
                );
            }

            return;
        }

        GetInput();

        UpdateAnimation();

        UpdateInvincibility();
    }

    // =========================
    // FIXED UPDATE
    // =========================

    private void FixedUpdate()
    {
        // ถ้าตายแล้ว
        if (isDead)
        {
            if (rb != null)
            {
                rb.linearVelocity =
                    Vector2.zero;
            }

            return;
        }

        // หยุดตอนเปิด UI
        if (isUIOpen)
        {
            if (rb != null)
            {
                rb.linearVelocity =
                    Vector2.zero;
            }

            return;
        }

        Move();
    }

    // =========================
    // UI LOCK
    // =========================

    public void SetUIOpen(bool value)
    {
        isUIOpen = value;

        if (isUIOpen)
        {
            movement = Vector2.zero;

            if (rb != null)
            {
                rb.linearVelocity =
                    Vector2.zero;
            }

            if (animator != null)
            {
                animator.SetBool(
                    "IsMoving",
                    false
                );
            }
        }
    }

    // =========================
    // INPUT
    // =========================

    private void GetInput()
    {
        movement.x =
            Input.GetAxisRaw("Horizontal");

        movement.y =
            Input.GetAxisRaw("Vertical");

        movement =
            movement.normalized;
    }

    // =========================
    // MOVEMENT
    // =========================

    private void Move()
    {
        if (rb == null)
            return;

        rb.MovePosition(
            rb.position +
            movement *
            moveSpeed *
            Time.fixedDeltaTime
        );
    }

    // =========================
    // ANIMATION
    // =========================

    private void UpdateAnimation()
    {
        if (animator == null)
            return;

        bool isMoving =
            movement.sqrMagnitude > 0.01f;

        animator.SetBool(
            "IsMoving",
            isMoving
        );

        animator.SetFloat(
            "MoveX",
            movement.x
        );

        animator.SetFloat(
            "MoveY",
            movement.y
        );
    }

    // =========================
    // INVINCIBILITY
    // =========================

    private void UpdateInvincibility()
    {
        if (invincibilityTimer > 0f)
        {
            invincibilityTimer -=
                Time.deltaTime;
        }
    }

    // =========================
    // BLACKOUT DAMAGE
    // =========================

    private void HandleBlackoutDamage()
    {
        if (lampManager == null)
        {
            lampManager =
                FindFirstObjectByType<LampManager>();

            if (lampManager == null)
                return;
        }

        // ถ้าเลือดหมดแล้ว
        if (currentHealth <= 0f)
        {
            blackoutDamageTimer = 0f;
            return;
        }

        // =========================================
        // ไฟยังไม่ดับ
        // =========================================

        if (lampManager.GetRemainingTime() > 0f)
        {
            blackoutDamageTimer = 0f;
            return;
        }

        // =========================================
        // ไฟดับ
        // =========================================

        blackoutDamageTimer +=
            Time.deltaTime;

        if (blackoutDamageTimer >=
            blackoutDamageInterval)
        {
            blackoutDamageTimer = 0f;

            TakeBlackoutDamage();
        }
    }

    // =========================
    // BLACKOUT DAMAGE
    // =========================

    private void TakeBlackoutDamage()
    {
        if (isDead)
            return;

        if (blackoutDamage <= 0f)
            return;

        currentHealth -=
            blackoutDamage;

        currentHealth =
            Mathf.Clamp(
                currentHealth,
                0f,
                maxHealth
            );

        Debug.Log(
            "BLACKOUT DAMAGE: -" +
            blackoutDamage +
            " | HP: " +
            currentHealth +
            " / " +
            maxHealth
        );

        // Hit Sound
        if (audioSource != null &&
            hitSound != null)
        {
            audioSource.PlayOneShot(
                hitSound
            );
        }

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

        // =========================================
        // DEATH
        // =========================================

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    // =========================
    // DAMAGE
    // =========================

    public void TakeDamage(
        float damage,
        Vector2 hitDirection)
    {
        // ตายแล้ว
        if (isDead)
            return;

        if (damage <= 0f)
            return;

        if (invincibilityTimer > 0f)
            return;

        currentHealth -= damage;

        // Hit Sound
        if (audioSource != null &&
            hitSound != null)
        {
            audioSource.PlayOneShot(
                hitSound
            );
        }

        currentHealth =
            Mathf.Clamp(
                currentHealth,
                0f,
                maxHealth
            );

        Debug.Log(
            "Player HP: " +
            currentHealth +
            " / " +
            maxHealth
        );

        invincibilityTimer =
            hitInvincibilityTime;

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

        // Knockback
        ApplyKnockback(
            hitDirection
        );

        // Die
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    // =========================
    // HEAL
    // =========================

    public void Heal(float amount)
    {
        if (amount <= 0f)
            return;

        if (currentHealth <= 0f)
            return;

        if (isDead)
            return;

        currentHealth +=
            amount;

        currentHealth =
            Mathf.Clamp(
                currentHealth,
                0f,
                maxHealth
            );

        Debug.Log(
            "Player Heal: +" +
            amount +
            " | HP: " +
            currentHealth +
            " / " +
            maxHealth
        );
    }

    // =========================
    // KNOCKBACK
    // =========================

    private void ApplyKnockback(
        Vector2 direction)
    {
        if (rb == null)
            return;

        if (direction.sqrMagnitude <= 0.001f)
            return;

        direction.Normalize();

        rb.linearVelocity =
            Vector2.zero;

        rb.linearVelocity =
            direction *
            knockbackForce;
    }

    // =========================
    // HIT FLASH
    // =========================

    private IEnumerator HitFlash()
    {
        if (spriteRenderer == null)
            yield break;

        spriteRenderer.color =
            Color.red;

        yield return new WaitForSeconds(
            hitFlashDuration
        );

        spriteRenderer.color =
            Color.white;

        hitFlashCoroutine = null;
    }

    // =========================
    // DIE
    // =========================

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        Debug.Log(
            "PLAYER DEAD"
        );

        // หยุดการเคลื่อนที่
        movement =
            Vector2.zero;

        if (rb != null)
        {
            rb.linearVelocity =
                Vector2.zero;
        }

        // เล่น Death Animation
        if (animator != null)
        {
            animator.SetBool(
                "IsMoving",
                false
            );

            animator.SetTrigger(
                "Die"
            );
        }

        // ปิด Collider
        Collider2D collider =
            GetComponent<Collider2D>();

        if (collider != null)
        {
            collider.enabled =
                false;
        }

        // รอ Animation เล่นจบ
        StartCoroutine(
            WaitForDeathAnimation()
        );
    }

    // =========================
    // WAIT DEATH ANIMATION
    // =========================

    private IEnumerator WaitForDeathAnimation()
    {
        // รอ 1 frame
        yield return null;

        if (animator != null)
        {
            // รอจนเข้า Player_Dead
            while (
                !animator
                    .GetCurrentAnimatorStateInfo(0)
                    .IsName("Player_Dead")
            )
            {
                yield return null;
            }

            // รอจน Animation เล่นจบ
            while (
                animator
                    .GetCurrentAnimatorStateInfo(0)
                    .normalizedTime < 1f
            )
            {
                yield return null;
            }
        }

        // Animation จบแล้ว
        // ค่อยขึ้น Game Over
        if (GameOverManager.Instance != null)
        {
            GameOverManager.Instance.ShowGameOver();
        }
    }

    // =========================
    // INCREASE MAX HEALTH
    // =========================

    public void IncreaseMaxHealth(
        float amount)
    {
        if (amount <= 0f)
            return;

        maxHealth +=
            amount;

        currentHealth +=
            amount;

        currentHealth =
            Mathf.Clamp(
                currentHealth,
                0f,
                maxHealth
            );

        Debug.Log(
            "Max Health เพิ่มเป็น: " +
            maxHealth
        );
    }
}