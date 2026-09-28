using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;

    [Header("Hit Effect")]
    [SerializeField] private float hitFlashDuration = 0.08f;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float hitInvincibilityTime = 0.5f;

    [Header("Player Sprite")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    private float currentHealth;
    private float invincibilityTimer;

    private Rigidbody2D rb;
    private Animator animator;

    private Vector2 movement;

    private Coroutine hitFlashCoroutine;


    // =====================================================
    // PUBLIC
    // =====================================================

    public float CurrentHealth => currentHealth;

    public float MaxHealth => maxHealth;


    // =====================================================
    // AWAKE
    // =====================================================

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();

        currentHealth = maxHealth;
    }


    // =====================================================
    // UPDATE
    // =====================================================

    private void Update()
    {
        GetInput();

        UpdateAnimation();

        UpdateInvincibility();
    }


    // =====================================================
    // FIXED UPDATE
    // =====================================================

    private void FixedUpdate()
    {
        Move();
    }


    // =====================================================
    // INPUT
    // =====================================================

    private void GetInput()
    {
        movement.x =
            Input.GetAxisRaw("Horizontal");

        movement.y =
            Input.GetAxisRaw("Vertical");

        movement =
            movement.normalized;
    }


    // =====================================================
    // MOVEMENT
    // =====================================================

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


    // =====================================================
    // ANIMATION
    // =====================================================

    private void UpdateAnimation()
    {
        if (animator == null)
            return;


        bool isMoving =
            movement.sqrMagnitude > 0.01f;


        // -----------------------------
        // MOVING / IDLE
        // -----------------------------

        animator.SetBool(
            "IsMoving",
            isMoving
        );


        // -----------------------------
        // MOVEMENT X
        // -----------------------------

        animator.SetFloat(
            "MoveX",
            movement.x
        );


        // -----------------------------
        // MOVEMENT Y
        // -----------------------------

        animator.SetFloat(
            "MoveY",
            movement.y
        );
    }


    // =====================================================
    // INVINCIBILITY
    // =====================================================

    private void UpdateInvincibility()
    {
        if (invincibilityTimer > 0f)
        {
            invincibilityTimer -=
                Time.deltaTime;
        }
    }


    // =====================================================
    // DAMAGE
    // =====================================================

    public void TakeDamage(
        float damage,
        Vector2 hitDirection
    )
    {
        if (damage <= 0f)
            return;


        // -----------------------------
        // INVINCIBILITY CHECK
        // -----------------------------

        if (invincibilityTimer > 0f)
            return;


        // -----------------------------
        // DAMAGE
        // -----------------------------

        currentHealth -= damage;


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


        // -----------------------------
        // INVINCIBILITY
        // -----------------------------

        invincibilityTimer =
            hitInvincibilityTime;


        // -----------------------------
        // HIT FLASH
        // -----------------------------

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


        // -----------------------------
        // KNOCKBACK
        // -----------------------------

        ApplyKnockback(
            hitDirection
        );


        // -----------------------------
        // DEATH
        // -----------------------------

        if (currentHealth <= 0f)
        {
            Die();
        }
    }


    // =====================================================
    // KNOCKBACK
    // =====================================================

    private void ApplyKnockback(
        Vector2 direction
    )
    {
        if (rb == null)
            return;


        if (direction.sqrMagnitude <= 0.001f)
            return;


        direction.Normalize();


        // ล้างความเร็วเดิม
        // ป้องกัน Knockback สะสม
        rb.linearVelocity =
            Vector2.zero;


        // ใส่ Knockback ใหม่
        rb.linearVelocity =
            direction *
            knockbackForce;
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
            hitFlashDuration
        );


        spriteRenderer.color =
            Color.white;


        hitFlashCoroutine = null;
    }


    // =====================================================
    // DEATH
    // =====================================================

    private void Die()
    {
        Debug.Log(
            "PLAYER DEAD"
        );


        // Game Over จะทำทีหลัง
    }
}